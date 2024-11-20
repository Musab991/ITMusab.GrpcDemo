#In this setup, two tasks run in parallel over a single gRPC connection: 
##one sends real-time PulseMessage heartbeats to keep the connection alive, and the other periodically sends TrackingMessage data updates.

#Benefits:
##Efficient Use of Resources: Both tasks share a single TCP connection, reducing overhead and making the system more resource-efficient.
##Strong Coupling and Reliability: Running both message types over one stream ensures consistent, real-time communication and keeps the client tightly synchronized with the server.
