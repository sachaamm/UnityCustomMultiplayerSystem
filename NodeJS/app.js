class SocketClient {
  constructor(c, id) {
    this.conn = c;
    this.uuid = id;
  }

  SetPositionAndRotation(x, y, z, rx, ry, rz) {
    this.posX = x;
    this.posY = y;
    this.posZ = z;
    this.rotX = rx;
    this.rotY = ry;
    this.rotZ = rz;
  }
}

// Include Nodejs' net module.
const Net = require("net");
// The port on which the server is listening.
const port = 8080;

// Use net.createServer() in your code. This is just for illustration purpose.
// Create a new TCP server.
const server = new Net.Server();

// const a = new SocketClient('a','b');

// console.log(a);

let clients = [];
// The server listens to a socket for a client to make a connection request.
// Think of a socket as an end point.
server.listen(port, function() {
  console.log("Server listening for connection requests on socket localhost:");
  console.log(port);
});

// When a client requests a connection with the server, the server creates a new
// socket dedicated to that client.
server.on("connection", function(socket) {
  console.log("A new connection has been established.");

  // CREATE UUID FOR SOCKET, AND STORE IT IN AN OBJECT.

  // Now that a TCP connection has been established, the server can send data to
  // the client by writing to its socket.
  // socket.write('Hello_client');
  // console.log(socket);

  let uuid = "";

  for (let i = 0; i < 9; i++) {
    const rd = parseInt(Math.random() * 10);
    uuid += rd;
  }

  socket.write("Hello_" + uuid);

  

  clients.forEach(socketClient => {
    // console.log(socketClient);
    // console.log(clients);

    console.log("client uuid " + socketClient.uuid);

    socketClient.conn.write("NewClient_" + uuid);
    // if (socketClient.uuid != uuid)
    socket.write("NewClient_" + socketClient.uuid);
  });

  clients.push(new SocketClient(socket, uuid));

  console.log("client pushed ");

  // The server can also receive data from the client by reading from its socket.
  socket.on("data", function(chunk) {
    console.log("data received from client:");

    const chunkMsg = chunk.toString();
    console.log(chunkMsg); // ON RECUPERE LA POSITION DU CLIENT

    const spl = chunkMsg.split("_");

    const msgLabel = spl[0];
    let clientUuid = null;

    if (spl.length > 0) clientUuid = spl[1];

    console.log("msgLabel " + msgLabel);

    switch (msgLabel) {
      case "NP": // New Player
        break;

      case "GT": // Get Transform
        clients.forEach(socketClient => {
          if (socketClient.uuid === clientUuid) {
            // console.log("same uuid !");

            // socketClient.SetPositionAndRotation(spl[1],spl[2],spl[2],spl[3],spl[4],spl[5]);
          } else {
            socketClient.conn.write(
              "GetPos_" +
                clientUuid +
                "_" +
                spl[2] +
                "_" +
                spl[3] +
                "_" +
                spl[4] +
                "_" +
                spl[5] +
                "_" +
                spl[6] +
                "_" +
                spl[7] +
                "_"
            ); // ON ENVOIE LA POSITION ET LA ROTATION DES AUTRES CLIENTS
          }
        });

        break;
    }

    // SEND DATA TO ALL OTHERS CLIENTS
  });

  // When the client requests to end the TCP connection with the server, the server
  // ends the connection.
  socket.on("end", function() {
    console.log("Closing connection with the client");
  });

  // Don't forget to catch error, for your own sake.
  socket.on("error", function(err) {
    console.log("Error ! ");
    console.log(err);
  });
});
