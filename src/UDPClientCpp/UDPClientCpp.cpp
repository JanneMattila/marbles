#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <cstring>
#include <chrono>
#include <thread>

#pragma comment(lib, "Ws2_32.lib")

int main()
{
    std::cout << "Waiting for 3 seconds before starting the client..." << std::endl;
    std::this_thread::sleep_for(std::chrono::seconds(3));

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

    sockaddr_in servaddr{};
    servaddr.sin_family = AF_INET;
    servaddr.sin_port = htons(5555);
    inet_pton(AF_INET, "127.0.0.1", &servaddr.sin_addr);

    // Send START message to server
    const char* startMessage = "START";
    if (sendto(sockfd, startMessage, strlen(startMessage), 0, (SOCKADDR*)&servaddr, sizeof(servaddr)) == SOCKET_ERROR) {
        std::cerr << "sendto failed with error: " << WSAGetLastError() << std::endl;
        closesocket(sockfd);
        WSACleanup();
        return 1;
    }

    // Wait for PING response
    char buffer[1024];
    int servlen = sizeof(servaddr);
    int n = recvfrom(sockfd, buffer, 1024, 0, (SOCKADDR*)&servaddr, &servlen);
    if (n == SOCKET_ERROR) {
        std::cerr << "recvfrom failed with error: " << WSAGetLastError() << std::endl;
        closesocket(sockfd);
        WSACleanup();
        return 1;
    }
    buffer[n] = '\0'; // Null-terminate the string

    if (strcmp(buffer, "PING") == 0) {
        std::cout << "Server: " << buffer << std::endl;
        // Wait for 1 second
        std::this_thread::sleep_for(std::chrono::seconds(1));

        // Send PONG response to server
        const char* pongMessage = "PONG";
        sendto(sockfd, pongMessage, strlen(pongMessage), 0, (SOCKADDR*)&servaddr, sizeof(servaddr));
    }
    else {
        std::cerr << "Unexpected response from server: " << buffer << std::endl;
        closesocket(sockfd);
        WSACleanup();
        return 1;
    }

    // Close the socket and cleanup
    closesocket(sockfd);
    WSACleanup();

    return 0;
}
