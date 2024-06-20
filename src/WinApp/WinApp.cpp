// WinApp.cpp : Defines the entry point for the application.
//

#include "framework.h"
#include "WinApp.h"
// DirectX Header Files
#include <d2d1.h>
#include <dwrite.h>

#include <chrono>
#include <windows.h>
#include <shellscalingapi.h>
#include <wincodec.h> // For WIC

#pragma comment(lib, "dwrite.lib")
#pragma comment(lib, "d2d1.lib")

#pragma comment(lib, "windowscodecs.lib")

#define MAX_LOADSTRING 100
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(x) { if ((x)) { (x)->Release(); (x) = nullptr; } }
#endif

// Global Variables:
HINSTANCE hInst;                                // current instance
HWND g_hwnd;                                      // Handle to the window
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name
bool g_bRunning = true;
// Global or class member variables
UINT32 g_frameCount = 0;
double g_lastTime = 0.0;
double g_fps = 0.0;

// Key state variables
bool g_keyUpPressed = false;
bool g_keyDownPressed = false;
bool g_keyLeftPressed = false;
bool g_keyRightPressed = false;
// Player position and size
D2D1_RECT_F g_playerRect = D2D1::RectF(100.f, 100.f, 30.f, 30.f);

// DirectX Global declarations
// Create a Direct2D render target
ID2D1HwndRenderTarget* g_pRenderTarget = nullptr;
// Initialize Direct2D Factory
ID2D1Factory* g_pD2DFactory = nullptr;
// Create a DirectWrite factory and text format object
IDWriteFactory* g_pDWriteFactory = nullptr;
IDWriteTextFormat* g_pTextFormat = nullptr;

ID2D1SolidColorBrush* g_pWhiteBrush = nullptr;
ID2D1SolidColorBrush* g_pBlueBrush = nullptr;

ID2D1Bitmap* g_pBitmap = nullptr;

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
HRESULT             InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);
void                CleanupDevice();
HRESULT             LoadPngFromResource(ID2D1RenderTarget* pRenderTarget, IWICImagingFactory* pIWICFactory, UINT resourceID, ID2D1Bitmap** pBitmap);
void                Render();
void                UpdatePlayerPosition();

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // Initialize global strings
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_WINAPP, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    HRESULT hr = InitInstance(hInstance, nCmdShow);
    if (FAILED(hr))
    {
        return FALSE;
    }

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_WINAPP));

    MSG msg = {};

    // Use high_resolution_clock for timing
    auto currentTime = std::chrono::high_resolution_clock::now();
    auto timeSpan = currentTime.time_since_epoch();
    double seconds = std::chrono::duration_cast<std::chrono::duration<double>>(timeSpan).count();

    while (g_bRunning)
    {
        // Process messages
        while (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }

        // Update game state
        UpdatePlayerPosition();

        // Render a frame
        Render();

        // Update frame count every frame
        g_frameCount++;

        // Calculate delta time
        auto newTime = std::chrono::high_resolution_clock::now();
        auto newTimeSpan = newTime.time_since_epoch();
        double newSeconds = std::chrono::duration_cast<std::chrono::duration<double>>(newTimeSpan).count();
        double deltaTime = newSeconds - g_lastTime;

        // Update FPS every second
        if (deltaTime >= 1.0)
        {
            g_fps = g_frameCount / deltaTime;
            g_frameCount = 0;
            g_lastTime = newSeconds;
        }
    }

    CoUninitialize();

    return (int) msg.wParam;
}

ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_WINAPP));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    //wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_WINAPP);
    wcex.lpszMenuName   = 0;
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

HRESULT InitDeviceResources(HWND hWnd)
{
    HRESULT hr = D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &g_pD2DFactory);
    if (FAILED(hr)) return hr;
    RECT rc;
    GetClientRect(hWnd, &rc); // hWnd is the handle to your application window

    D2D1_SIZE_U size = D2D1::SizeU(rc.right - rc.left, rc.bottom - rc.top);

    hr = g_pD2DFactory->CreateHwndRenderTarget(
        D2D1::RenderTargetProperties(),
        D2D1::HwndRenderTargetProperties(hWnd, size),
        &g_pRenderTarget);
    if (FAILED(hr)) return hr;

    hr = g_pRenderTarget->CreateSolidColorBrush(
        D2D1::ColorF(D2D1::ColorF::White),
        &g_pWhiteBrush);
    if (FAILED(hr)) return hr;

    hr = g_pRenderTarget->CreateSolidColorBrush(
        D2D1::ColorF(D2D1::ColorF::Blue),
        &g_pBlueBrush);
    if (FAILED(hr)) return hr;

    // Create a DirectWrite factory and text format object
    DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(IDWriteFactory),
        reinterpret_cast<IUnknown**>(&g_pDWriteFactory));

    g_pDWriteFactory->CreateTextFormat(
        L"Arial",                // Font family name
        nullptr,                 // Font collection (NULL sets it to the system font collection)
        DWRITE_FONT_WEIGHT_NORMAL,
        DWRITE_FONT_STYLE_NORMAL,
        DWRITE_FONT_STRETCH_NORMAL,
        20.0f,                   // Font size
        L"",                     // Locale
        &g_pTextFormat);

    // Set alignment
    g_pTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING);
    g_pTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR);

    // Initialize COM library with STA concurrency model.
    hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    if (FAILED(hr)) return hr;

    // Load a PNG image from the resources
    IWICImagingFactory* pIWICFactory = nullptr;
    hr = CoCreateInstance(CLSID_WICImagingFactory, nullptr, CLSCTX_INPROC_SERVER, IID_IWICImagingFactory, reinterpret_cast<void**>(&pIWICFactory));
    if (FAILED(hr)) return hr;

    hr = LoadPngFromResource(g_pRenderTarget, pIWICFactory, IDB_PNG_CAR, &g_pBitmap);
    if (FAILED(hr)) return hr;

	return hr;
}

HRESULT InitInstance(HINSTANCE hInstance, int nCmdShow)
{
    hInst = hInstance; // Store instance handle in our global variable

    HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

    if (!hWnd) return E_FAIL;

    HRESULT hr = InitDeviceResources(hWnd);

    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);

    g_hwnd = hWnd;
    return hr;
}

void UpdatePlayerPosition()
{
    if (g_keyUpPressed) { g_playerRect.top -= 10; g_playerRect.bottom -= 10; }
    if (g_keyDownPressed) { g_playerRect.top += 10; g_playerRect.bottom += 10; }
    if (g_keyLeftPressed) { g_playerRect.left -= 10; g_playerRect.right -= 10; }
    if (g_keyRightPressed) { g_playerRect.left += 10; g_playerRect.right += 10; }
}

void Render()
{
    if (g_pRenderTarget == nullptr) return;

    g_pRenderTarget->BeginDraw();
    g_pRenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::Black));

    // Draw the bitmap
	//g_pRenderTarget->DrawBitmap(g_pBitmap);

    // Define the destination rectangle for the bitmap on the render target
    D2D1_RECT_F destRect = D2D1::RectF(0.0f, 0.0f, g_pBitmap->GetSize().width, g_pBitmap->GetSize().height);

    // Draw the bitmap
    g_pRenderTarget->DrawBitmap(g_pBitmap, destRect);

    // Use g_playerRect for the player's position and size
    g_pRenderTarget->FillRectangle(&g_playerRect, g_pBlueBrush);

    wchar_t fpsText[256];
    swprintf_s(fpsText, L"FPS: %.2lf", g_fps);

    // Draw the text
    g_pRenderTarget->DrawText(
        fpsText,
        wcslen(fpsText),
        g_pTextFormat,
        D2D1::RectF(0, 0, 200, 50), // Position and size of the text
        g_pWhiteBrush);

    g_pRenderTarget->EndDraw();
    HRESULT hr = g_pRenderTarget->EndDraw();
    if (hr == D2DERR_RECREATE_TARGET)
    {
        CleanupDevice();

        // Device lost, recreate device resources
        InitDeviceResources(g_hwnd);
    }
}

HRESULT LoadPngFromResource(ID2D1RenderTarget* pRenderTarget, IWICImagingFactory* pIWICFactory, UINT resourceID, ID2D1Bitmap** pBitmap)
{
    HRSRC imageResHandle = nullptr;
    HGLOBAL imageResDataHandle = nullptr;
    void* pImageFile = nullptr;
    DWORD imageFileSize = 0;

    *pBitmap = nullptr;

    // Locate the resource.
    imageResHandle = FindResource(hInst, MAKEINTRESOURCE(resourceID), L"PNG");
    if (!imageResHandle) return E_FAIL;

    // Load the resource.
    imageResDataHandle = LoadResource(hInst, imageResHandle);
    if (!imageResDataHandle) return E_FAIL;

    // Lock it to get a pointer to the resource data.
    pImageFile = LockResource(imageResDataHandle);
    imageFileSize = SizeofResource(hInst, imageResHandle);

    // Create a WIC stream to read the image.
    IWICStream* pStream = nullptr;
    HRESULT hr = pIWICFactory->CreateStream(&pStream);
    if (FAILED(hr)) return hr;

    hr = pStream->InitializeFromMemory(reinterpret_cast<BYTE*>(pImageFile), imageFileSize);
    if (FAILED(hr)) return hr;

    // Create a decoder for the stream.
    IWICBitmapDecoder* pDecoder = nullptr;
    hr = pIWICFactory->CreateDecoderFromStream(pStream, nullptr, WICDecodeMetadataCacheOnLoad, &pDecoder);
    if (FAILED(hr)) return hr;

    // Read the first frame of the image.
    IWICBitmapFrameDecode* pFrame = nullptr;
    hr = pDecoder->GetFrame(0, &pFrame);
    if (FAILED(hr)) return hr;

    // Convert the image to a pixel format that Direct2D expects
    IWICFormatConverter* pConverter = nullptr;
    hr = pIWICFactory->CreateFormatConverter(&pConverter);
    if (FAILED(hr)) return hr;

    hr = pConverter->Initialize(
        pFrame,                          // Input bitmap to convert
        GUID_WICPixelFormat32bppPBGRA,   // Destination pixel format
        WICBitmapDitherTypeNone,         // Specified dither pattern
        nullptr,                         // Specify a particular palette
        0.f,                             // Alpha threshold
        WICBitmapPaletteTypeCustom       // Palette translation type
    );
    if (FAILED(hr)) return hr;

    // Convert the frame to a Direct2D bitmap.
    hr = pRenderTarget->CreateBitmapFromWicBitmap(pConverter, nullptr, pBitmap);
    if (FAILED(hr)) return hr;
    if (*pBitmap == nullptr) return E_FAIL;

    // Clean up.
    SAFE_RELEASE(pConverter);
    SAFE_RELEASE(pFrame);
    SAFE_RELEASE(pDecoder);
    SAFE_RELEASE(pStream);
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    //case WM_KEYDOWN:
    //    if (wParam == VK_RETURN && (GetAsyncKeyState(VK_SHIFT) & 0x8000)) {
    //        static bool isFullscreen = false; // Tracks the current fullscreen state
    //        isFullscreen = !isFullscreen; // Toggle state
    //        static WINDOWPLACEMENT windowPlacement = { sizeof(windowPlacement) };

    //        if (isFullscreen) {
    //            // Store the current window dimensions for restoring later
    //            GetWindowPlacement(hWnd, &windowPlacement);

    //            // Get the dimensions of the primary monitor
    //            MONITORINFO monitorInfo = { sizeof(monitorInfo) };
    //            GetMonitorInfo(MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY), &monitorInfo);

    //            // Set window style for fullscreen and position it
    //            SetWindowLong(hWnd, GWL_STYLE, WS_POPUP);
    //            SetWindowPos(hWnd, HWND_TOP,
    //                monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top,
    //                monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left,
    //                monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top,
    //                SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
    //        }
    //        else {
    //            // Restore the window to its previous dimensions
    //            SetWindowLong(hWnd, GWL_STYLE, WS_OVERLAPPEDWINDOW);
    //            SetWindowPlacement(hWnd, &windowPlacement);
    //            SetWindowPos(hWnd, NULL, 0, 0, 0, 0,
    //                SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER |
    //                SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
    //        }
    //    }
    //    break;

    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // Parse the menu selections:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        }
        break;
    case WM_KEYDOWN:
        switch (wParam)
        {
        case VK_UP:    g_keyUpPressed = true; break;
        case VK_DOWN:  g_keyDownPressed = true; break;
        case VK_LEFT:  g_keyLeftPressed = true; break;
        case VK_RIGHT: g_keyRightPressed = true; break;
        }
        break;

    case WM_KEYUP:
        switch (wParam)
        {
        case VK_UP:    g_keyUpPressed = false; break;
        case VK_DOWN:  g_keyDownPressed = false; break;
        case VK_LEFT:  g_keyLeftPressed = false; break;
        case VK_RIGHT: g_keyRightPressed = false; break;
        }
        break;

    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);
            Render();
            EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        CleanupDevice();
        g_bRunning = false; // This will cause the game loop to exit
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}

void CleanupDevice()
{
    SAFE_RELEASE(g_pWhiteBrush);
    SAFE_RELEASE(g_pBlueBrush);
    SAFE_RELEASE(g_pBitmap);
    SAFE_RELEASE(g_pRenderTarget);
    SAFE_RELEASE(g_pD2DFactory);
    SAFE_RELEASE(g_pTextFormat);
    SAFE_RELEASE(g_pDWriteFactory);
}
