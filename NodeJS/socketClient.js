class SocketClient{

    // conn, uuid
    conn;
    uuid;

    posX;
    posY;
    posZ;
    rotX;
    rotY;
    rotZ;

    SocketClient(c,id){
        this.conn = c;
        this.uuid = id;
    }

    SetPositionAndRotation(x,y,z,rx,ry,rz){
        this.posX = x;
        this.posY = y;
        this.posZ = z;
        this.rotX = rx;
        this.rotY = ry;
        this.rotZ = rz;
    }


}