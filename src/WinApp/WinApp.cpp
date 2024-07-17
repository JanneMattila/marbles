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

#include <cmath> // Include the <cmath> header for mathematical functions
#include <string>
#include <map>
#include <vector>

#pragma comment(lib, "dwrite.lib")
#pragma comment(lib, "d2d1.lib")

#pragma comment(lib, "windowscodecs.lib")

#define MAX_LOADSTRING 100
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(x) { if ((x)) { (x)->Release(); (x) = nullptr; } }
#endif

#ifndef M_PI
#define M_PI 3.14159265358979323846
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
D2D1_POINT_2F g_playerPosition = D2D1::Point2F(100.f, 100.f);
double g_playerRotation = 0.0;
double g_wheelAngle = 0.0; // Wheel angle in radians, relative to the car's orientation
double g_playerRotationSpeed = 0.0;
double g_playerSpeed = 0.0;

// DirectX Global declarations
// Create a Direct2D render target
ID2D1HwndRenderTarget* g_pRenderTarget = nullptr;
// Initialize Direct2D Factory
ID2D1Factory* g_pD2DFactory = nullptr;
// Create a DirectWrite factory and text format object
IDWriteFactory* g_pDWriteFactory = nullptr;
IDWriteTextFormat* g_pTextFormat = nullptr;

ID2D1SolidColorBrush* g_pWhiteBrush = nullptr;
ID2D1SolidColorBrush* g_pGrayBrush = nullptr;
ID2D1SolidColorBrush* g_pBlueBrush = nullptr;

ID2D1Bitmap* g_pCarBitmap = nullptr;
ID2D1Bitmap* g_pExplosionBitmap = nullptr;

const int totalFrames = 48; // Total frames in the sprite sheet
const int frameWidth = 240; // Width of each frame
const int frameHeight = 240; // Height of each frame
float frameDuration = 0.08f; // Duration of each frame in seconds

struct Explosion {
    D2D1_POINT_2F position;
    int currentFrame = 0;
    float frameTime = frameDuration; // Time until the next frame
    bool isActive = true; // Whether the explosion is active or has finished
};

std::vector<Explosion> explosions;

struct SmokeParticle {
    D2D1_POINT_2F position;
    D2D1_POINT_2F velocity;
    float size;
    float lifespan; // Time remaining before the particle disappears
};

std::vector<SmokeParticle> smokeParticles;

struct TireMark {
    D2D1_POINT_2F startPoint;
    D2D1_POINT_2F endPoint;
    // Additional properties like width or color can be added here
};

std::vector<TireMark> tireMarks;

enum class RoadSurface {
    Asphalt,
    Gravel,
    Ice
};

// Traction coefficients for different surfaces
const std::map<RoadSurface, double> tractionCoefficients = {
    {RoadSurface::Asphalt, 1.0}, // High traction
    {RoadSurface::Gravel, 0.7},  // Medium traction
    {RoadSurface::Ice, 0.3}      // Low traction
};

// Assume the car starts on Asphalt
RoadSurface currentSurface = RoadSurface::Asphalt;


// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
HRESULT             InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);
void                CleanupDevice();
HRESULT             LoadPngFromResource(ID2D1RenderTarget* pRenderTarget, IWICImagingFactory* pIWICFactory, UINT resourceID, ID2D1Bitmap** pBitmap);
void                Render(double deltaTime);
void                UpdatePlayerPosition(double deltaTime);

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
        // Display an error message box
        MessageBox(NULL, L"Failed to initialize the application.", L"Error", MB_ICONERROR | MB_OK);
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
        // Update frame count every frame
        g_frameCount++;

        // Calculate delta time
        auto newTime = std::chrono::high_resolution_clock::now();
        auto newTimeSpan = newTime.time_since_epoch();
        double newSeconds = std::chrono::duration_cast<std::chrono::duration<double>>(newTimeSpan).count();
        double runningTime = newSeconds - g_lastTime;
        double deltaTime = newSeconds - seconds;
        seconds = newSeconds;
        // Update FPS every second
        if (runningTime >= 1.0)
        {
            g_fps = g_frameCount / runningTime;
            g_frameCount = 0;
            g_lastTime = newSeconds;
        }

        // Process messages
        while (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }

        // Update game state
        UpdatePlayerPosition(deltaTime);

        // Render a frame
        Render(deltaTime);
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
        D2D1::ColorF(D2D1::ColorF::Gray),
        &g_pGrayBrush);
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
        10.0f,                   // Font size
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

    // bing.com/create
    hr = LoadPngFromResource(g_pRenderTarget, pIWICFactory, IDB_PNG_CAR, &g_pCarBitmap);
    if (FAILED(hr)) return hr;

    g_playerRect = D2D1::RectF(0.0f, 0.0f, g_pCarBitmap->GetSize().width / 4, g_pCarBitmap->GetSize().height / 4);
    //g_playerPosition = D2D1::Point2F(size.width / 2, size.height / 2);
    g_playerPosition = D2D1::Point2F(100, 100);

    // https://www.hiclipart.com/free-transparent-background-png-clipart-plufo
    hr = LoadPngFromResource(g_pRenderTarget, pIWICFactory, IDB_PNG_EXPLOSION_ANIMATION, &g_pExplosionBitmap);
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

void EmitSmokeParticles(const D2D1_POINT_2F& carPosition, float carRotation) {
    SmokeParticle particle;
    particle.position = carPosition; // Start at the car's position
    particle.velocity = D2D1::Point2F(cos(carRotation) * -1.0f, sin(carRotation) * -1.0f); // Move opposite to car's direction
    particle.size = 5.0f; // Initial size of the smoke particle
    particle.lifespan = 1.0f; // 1 second lifespan

    smokeParticles.push_back(particle);
}

void UpdateSmokeParticles(float deltaTime) {
    for (auto& particle : smokeParticles) {
        particle.position.x += particle.velocity.x * deltaTime;
        particle.position.y += particle.velocity.y * deltaTime;
        particle.lifespan -= deltaTime;
    }

    // Remove dead particles
    smokeParticles.erase(std::remove_if(smokeParticles.begin(), smokeParticles.end(),
        [](const SmokeParticle& p) { return p.lifespan <= 0; }),
        smokeParticles.end());
}

void UpdateExplosions(float deltaTime) {
    for (auto& explosion : explosions) {
        if (!explosion.isActive) continue;

        explosion.frameTime -= deltaTime;
        if (explosion.frameTime <= 0) {
            explosion.currentFrame++;
            explosion.frameTime = frameDuration; // Reset timer for the next frame, adjust for desired frame rate

            if (explosion.currentFrame >= totalFrames) {
                explosion.isActive = false; // Animation finished
            }
        }
    }

    // Remove inactive explosions
    explosions.erase(std::remove_if(explosions.begin(), explosions.end(),
        [](const Explosion& e) { return !e.isActive; }),
         explosions.end());
}

void AddExplosion(const D2D1_POINT_2F& position) {
    Explosion newExplosion;
    newExplosion.position = position;
    explosions.push_back(newExplosion);

    // Debug output
    wchar_t debugMessage[256];
    swprintf_s(debugMessage, L"Explosion added at position: (%f, %f)\n", position.x, position.y);
    OutputDebugString(debugMessage);
}

void UpdatePlayerPosition(double deltaTime)
{
    double traction = tractionCoefficients.at(currentSurface);

    const double maxSpeed = 3.0f; // Max speed in units per second
    const double maxSpeedReverse = 2.0f;
    const double maxRotationAngle = 4.0f;
    const double acceleration = 10.0f * traction;
    const double deacceleration = 5.0f * traction;
    const double rotationAcceleration = 0.5f * traction;
    const double rotationDeacceleration = 0.9 * traction;

    const double dragCoefficient = 0.05; // Drag coefficient for natural deceleration
    const double turnSensitivity = 0.5 * traction; // Sensitivity of turning based on speed
    double driftFactor = 0.1; // Factor controlling the amount of slide during a turn

    double turnRate = maxSpeed / (fabs(g_playerSpeed) + 1) * turnSensitivity;
    bool isTurning = false;

    // Determine if the car is attempting a quick turn
    bool isQuickTurn = fabs(g_wheelAngle) > 0.2; // Example condition for quick turns

    // Adjust the drift factor based on conditions (e.g., road surface, speed)
    if (isQuickTurn && g_playerSpeed > maxSpeed / 2.0) // Example condition for sliding
    {
        driftFactor += 0.5; // Increase drift factor for more slide
        EmitSmokeParticles(g_playerPosition, g_playerRotation);
    }
    else
    {
        driftFactor = 0.1; // Reset to default drift factor
    }

    // Handle keyboard input for rotation
    if (g_playerSpeed >= 0)
    {
        if (g_keyLeftPressed)
        {
            isTurning = true;
            g_wheelAngle -= turnRate;
            if (g_wheelAngle < -maxRotationAngle)
            {
                g_wheelAngle = -maxRotationAngle;
            }
        }
        if (g_keyRightPressed)
        {
            isTurning = true;
            g_wheelAngle += turnRate;
            if (g_wheelAngle > maxRotationAngle)
            {
                g_wheelAngle = maxRotationAngle;
            }
        }
    }
    else if (g_playerSpeed < 0)
    {
        if (g_keyLeftPressed)
        {
            g_wheelAngle += turnRate;
            if (g_wheelAngle > maxRotationAngle)
            {
                g_wheelAngle = maxRotationAngle;
            }
        }
        if (g_keyRightPressed)
        {
            g_wheelAngle -= turnRate;
            if (g_wheelAngle < -maxRotationAngle)
            {
                g_wheelAngle = -maxRotationAngle;
            }
        }
    }

    // Deaccelerate if no left or right keys are pressed
    if (!g_keyLeftPressed && !g_keyRightPressed)
    {
        g_wheelAngle *= rotationDeacceleration;
    }

    // Handle keyboard input for speed
    if (g_keyDownPressed)
    {
        if (g_playerSpeed > 0)
        {
            g_playerSpeed -= acceleration * deltaTime;
            if (g_playerSpeed < 0.0)
            {
                g_playerSpeed = 0.0;
            }
        }
        else
        {
            g_playerSpeed -= acceleration * deltaTime;
            if (g_playerSpeed < -maxSpeedReverse)
            {
                g_playerSpeed = -maxSpeedReverse;
            }
        }

    }
    else if (g_keyUpPressed)
    {
        g_playerSpeed += acceleration * deltaTime;
        if (g_playerSpeed > maxSpeed)
        {
            g_playerSpeed = maxSpeed;
        }
    }
    else
    {
        // Deaccelerate if no up or down keys are pressed
        if (g_playerSpeed > 0.0)
        {
            g_playerSpeed -= deacceleration * deltaTime;
            if (g_playerSpeed < 0.0)
            {
                g_playerSpeed = 0.0;
            }
        }
        else if (g_playerSpeed < 0.0)
        {
            g_playerSpeed += deacceleration * deltaTime;
            if (g_playerSpeed > 0.0)
            {
                g_playerSpeed = 0.0;
            }
        }
    }

    if (g_playerSpeed != 0.0)
    {
        // Calculate the car's new orientation based on the wheel angle
        // This simulates the car gradually aligning with the direction of the wheels
        auto wheel = g_wheelAngle * deltaTime * (1.0 - driftFactor);
        g_playerRotation += wheel;

        // Calculate movement direction based on the car's orientation and wheel angle
        //double movementDirection = g_playerRotation + wheel + (isTurning ? driftFactor : 0.0);
        double movementDirection = g_playerRotation + wheel + (isTurning ? driftFactor : 0.0);

        // Calculate the new position based on the movement direction
        float moveX = g_playerSpeed * cos(movementDirection);
        float moveY = g_playerSpeed * sin(movementDirection);

        g_playerPosition.x += moveX;
        g_playerPosition.y += moveY;

        // Example of adding a tire mark when the car is drifting
        //if (isDrifting) {
            TireMark mark;
            mark.startPoint = g_playerPosition; // Assuming this is the center of the car
            mark.endPoint = D2D1::Point2F(g_playerPosition.x + cos(g_playerRotation) * 10, g_playerPosition.y + sin(g_playerRotation) * 10); // Example endpoint calculation
            tireMarks.push_back(mark);
        //}
    }

    if (explosions.empty())
    {
        AddExplosion(D2D1::Point2F(100.0f, 100.0f)); // Example position
    }

    UpdateExplosions(deltaTime);
    UpdateSmokeParticles(deltaTime);
}

void RenderExplosions(ID2D1HwndRenderTarget* pRenderTarget, ID2D1Bitmap* pSpriteSheet) {
    for (const auto& explosion : explosions) {
        if (!explosion.isActive) continue;

        // Calculate the column and row based on the current frame
        int column = explosion.currentFrame % 8; // 8 columns
        int row = explosion.currentFrame / 8; // 6 rows

        // Calculate the source rectangle of the current frame
        D2D1_RECT_F srcRect = D2D1::RectF(
            frameWidth * column, frameHeight * row,
            frameWidth * (column + 1), frameHeight * (row + 1));

        // Destination rectangle on the screen
        D2D1_RECT_F destRect = D2D1::RectF(
            explosion.position.x, explosion.position.y,
            explosion.position.x + frameWidth, explosion.position.y + frameHeight);

        // Draw the current frame
        pRenderTarget->DrawBitmap(pSpriteSheet, destRect, 1.0f, D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, srcRect);
    }
}


void Render(double deltaTime)
{
    if (g_pRenderTarget == nullptr) return;

    g_pRenderTarget->BeginDraw();
    g_pRenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::Black));

    for (const auto& mark : tireMarks) {
        g_pRenderTarget->DrawLine(mark.startPoint, mark.endPoint, g_pWhiteBrush, 2.0f); // Draw each tire mark as a line
    }

    // Draw the bitmap
    D2D1_RECT_F destRect = D2D1::RectF(
        g_playerPosition.x, g_playerPosition.y,
        g_playerPosition.x + g_playerRect.right, g_playerPosition.y + g_playerRect.bottom);

    // Calculate the center point of the bitmap
    D2D1_POINT_2F center = D2D1::Point2F(
        g_playerPosition.x + g_playerRect.right / 2,
        g_playerPosition.y + g_playerRect.bottom / 2);

    // Rotate the bitmap based on the player rotation
    float degrees = g_playerRotation * (180.0 / M_PI);
    g_pRenderTarget->SetTransform(D2D1::Matrix3x2F::Rotation(static_cast<float>(degrees + 90), center));

    g_pRenderTarget->DrawBitmap(g_pCarBitmap, destRect);

    // Reset the transform
    g_pRenderTarget->SetTransform(D2D1::Matrix3x2F::Identity());

    for (const auto& particle : smokeParticles) {
        // Draw each particle as a circle
        D2D1_ELLIPSE ellipse = D2D1::Ellipse(particle.position, particle.size, particle.size);
        g_pRenderTarget->FillEllipse(&ellipse, g_pGrayBrush);
    }

    RenderExplosions(g_pRenderTarget, g_pExplosionBitmap);

    // Use g_playerRect for the player's position and size
    //g_pRenderTarget->FillRectangle(&g_playerRect, g_pBlueBrush);

    wchar_t fpsText[256];
    swprintf_s(fpsText, L"FPS: %.2lf", g_fps);

    // Draw the text
    g_pRenderTarget->DrawText(
        fpsText,
        wcslen(fpsText),
        g_pTextFormat,
        D2D1::RectF(0, 0, 200, 50), // Position and size of the text
        g_pWhiteBrush);

    // Draw the keyboard state
    std::wstring text = L"Delta time:\n";
    text += std::to_wstring(deltaTime);
    text += L"\nSpeed:\n";
    text += std::to_wstring(g_playerSpeed);
    text += L"\nRotation:\n";
    text += std::to_wstring(g_playerRotation);
    text += L"\nKeys pressed: ";
    if (g_keyUpPressed)
    {
        text += L"Up ";
    }
    if (g_keyDownPressed)
    {
        text += L"Down ";
    }
    if (g_keyLeftPressed)
    {
        text += L"Left ";
    }
    if (g_keyRightPressed)
    {
        text += L"Right ";
    }

    g_pRenderTarget->DrawText(
        text.c_str(),
        text.length(),
        g_pTextFormat,
        D2D1::RectF(0, 50, 200, 100), // Position and size of the text
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
            //Render();
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
    SAFE_RELEASE(g_pGrayBrush);
    SAFE_RELEASE(g_pBlueBrush);
    SAFE_RELEASE(g_pCarBitmap);
    SAFE_RELEASE(g_pExplosionBitmap);
    SAFE_RELEASE(g_pRenderTarget);
    SAFE_RELEASE(g_pD2DFactory);
    SAFE_RELEASE(g_pTextFormat);
    SAFE_RELEASE(g_pDWriteFactory);
}
