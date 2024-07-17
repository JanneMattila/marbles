#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h> // Include Windows.h for GetPrivateProfileString
#include <cwchar>

#pragma comment(lib, "Ws2_32.lib")

int main()
{
    WSADATA wsaData;
    int iResult;

    // Initialize Winsock
    iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0) {
        std::cerr << "WSAStartup failed: " << iResult << std::endl;
        return 1;
    }

    SOCKET sockfd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (sockfd == INVALID_SOCKET) {
        std::cerr << "Socket creation failed with error: " << WSAGetLastError() << std::endl;
        WSACleanup();
        return 1;
    }

    // Read port number from config.ini
    wchar_t exePath[MAX_PATH];
    GetModuleFileName(NULL, exePath, MAX_PATH);

    // Extract directory from exePath
    wchar_t* lastSlash = wcsrchr(exePath, L'\\');
    if (lastSlash) {
        *(lastSlash + 1) = L'\0'; // Terminate the string right after the last slash
    }

    // Append config.ini to the directory path
    wcscat_s(exePath, MAX_PATH, L"config.ini");
    std::wcout << exePath << std::endl;

    // Now exePath contains the full path to config.ini
    wchar_t portStr[6];
    GetPrivateProfileString(L"Settings", L"Port", L"5556", portStr, 6, exePath);

    int port = std::wcstol(portStr, nullptr, 10); // Convert port number string to integer

    sockaddr_in servaddr{};
    servaddr.sin_family = AF_INET; // IPv4
    servaddr.sin_addr.s_addr = htonl(INADDR_ANY); // Any incoming interface
    servaddr.sin_port = htons(port); // Port number read from config.ini

    // Bind the socket with the server address
    if (bind(sockfd, (SOCKADDR*)&servaddr, sizeof(servaddr)) == SOCKET_ERROR) {
        std::cerr << "Bind failed with error: " << WSAGetLastError() << std::endl;
        closesocket(sockfd);
        WSACleanup();
        return 1;
    }

    std::cout << "UDP Server listening on port " << port << std::endl;

    while (true) {
        char buffer[1024];
        sockaddr_in cliaddr;
        int cliaddrlen = sizeof(cliaddr);

        // Receive message from client
        int n = recvfrom(sockfd, buffer, 1024, 0, (SOCKADDR*)&cliaddr, &cliaddrlen);
        if (n == SOCKET_ERROR) {
            std::cerr << "recvfrom failed with error: " << WSAGetLastError() << std::endl;
            continue; // Skip this iteration
        }
        buffer[n] = '\0'; // Null-terminate the string

        std::cout << "Client: " << buffer << std::endl;

        // Send PING response to client
        const char* message = "PING";
        sendto(sockfd, message, strlen(message), 0, (SOCKADDR*)&cliaddr, sizeof(cliaddr));

        std::cout << "PING message sent." << std::endl;
    }

    // Close the socket and cleanup
    closesocket(sockfd);
    WSACleanup();

    return 0;
}
