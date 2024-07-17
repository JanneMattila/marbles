#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>

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

    sockaddr_in servaddr{};
    servaddr.sin_family = AF_INET; // IPv4
    servaddr.sin_addr.s_addr = htonl(INADDR_ANY); // Any incoming interface
    servaddr.sin_port = htons(5555); // Port number

    // Bind the socket with the server address
    if (bind(sockfd, (SOCKADDR*)&servaddr, sizeof(servaddr)) == SOCKET_ERROR) {
        std::cerr << "Bind failed with error: " << WSAGetLastError() << std::endl;
        closesocket(sockfd);
        WSACleanup();
        return 1;
    }

    std::cout << "UDP Server listening on port 5555" << std::endl;

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
