# DIDA-GStore
In this project we developed two versions of a geo-replicated storage system. A base version with a strong consistency but low availability and an advanced version with eventual consistency and high availability.

## System Architecture
The architecture of the system is based on partial replication. The system is composed of servers and partitions, where each server might be part of multiple partitions, and each partition might contain multiple servers. A write request to one partition is replicated across all servers of that partition. Two versions of this architecture were developed: a base version and an advanced version.

The base version of the system takes the master-slave approach and offers linearizability, with the drawback of having slow write requests. In the advanced version all servers have the same role, with all servers being allowed to write. It offers eventual consistency, with the advantage of having fast writes. This project was implemented using C# with an async/await programming style, and all methods that can run asynchronously will do so to avoid using resources while waiting. For this reason, no explicit thread pool or request queue was implemented. In the case of the server, when a request arrives, a task is automatically created by gRPC/C#.

## Base Version (branch)
The base version takes the master-slave approach and offers linearizability. Each partition contains one master server, while the remaining servers are replicas. Masters for one partition may be replicas for another, and vice-versa. Distributed locking is used between the masters and the replicas to ensure linearizability. This version compromises write speeds but ensures the data is always consistent. A write request takes six communication steps to complete (i.e., two between the client and the master, and four between the master and the replicas) while a read request takes two (i.e., two between the client and the master). Blocking time in either operation might be significant due to exclusive object locking in write operations.

- Write requests are sent to the master server. The master server will issue object-level write-locks to the replicas. Once a majority is locked, the master server will send the new value to the replicas and unlock them, write its own value and unlock itself.
- 
![image](https://user-images.githubusercontent.com/22732776/114277393-e830a600-9a22-11eb-9435-a472a02a3e41.png)

- Read requests are issued to any server. Read requests will read-lock at the object level. If a lock is granted, the read will be performed and the value will be returned to the client.

![image](https://user-images.githubusercontent.com/22732776/114277389-dc44e400-9a22-11eb-9d3e-e4daf491a501.png)

### Crash Recovery
The following diagram shows a representation of the crash recovery protocol.

![image](https://user-images.githubusercontent.com/22732776/114277641-e61b1700-9a23-11eb-80c2-a403ed1bab18.png)

## Advanced Version - No master (branch)
In the advanced version of the protocol there are no masters in the partitions. This allows the clients to send their read and write requests to any server in the partition. In this version of the protocol, updates to the objects are done in one server and are propagated to the others afterwards, guaranteeing that eventually all the servers in the system will get the most recent version of the objects, making the system eventually consistent. This version offers high availability and eventual consistency based on object versioning and local locks. Both write and read requests take two communication steps to complete (i.e., two between the client and the master).

- Write requests may be sent to any server of the partition. When a server receives a write request, it acquires a write lock on the object, updates the object, releases the lock, brodcasts the update to the other servers in the partition and returns to the client without waiting for the confirmation of the servers.

![image](https://user-images.githubusercontent.com/22732776/114277708-427e3680-9a24-11eb-8692-0a2ec8f8a2f3.png)

## Results

![image](https://user-images.githubusercontent.com/22732776/114277738-6b063080-9a24-11eb-815f-e3e7b0b4fa07.png)
![image](https://user-images.githubusercontent.com/22732776/114277743-7194a800-9a24-11eb-9f3c-abf0d8e6a0bf.png)
![image](https://user-images.githubusercontent.com/22732776/114277756-79544c80-9a24-11eb-9961-235561bc8a49.png)


## Developement and Testing

Script notes:
1. Included scripts have comments included within the script (which start with #).

Notes for running the project:
1. Build everything as debug. The current setup assumes the client and server .exe files are within the bin/Debug subfolders of their respective folders.
2. All client scripts must be placed in the same folder as the PCS .exe file.
3. All Puppetmaster scripts must be placed in the same folder as the Puppetmaster .exe file.
4. Only .txt files can be used for scripts. (If you want to call a script file inside a script don't provide the extension type - it will be automatically appended)
5. The puppetmaster must be shut down before starting a new configuration script.

Other remarks:
1. Servers are only created after the first non configuration command. It's advised to use wait X to let the servers start.
2. Servers are only aware of crashes if they have a master - replica relationship. Which means status command might print a "wrong" notion of dead or alive.
3. ReplicationFactor command is ignored. It's assumed that partition commands always use the same value for r.

Puppetmaster script run order (restart Puppetmaster after each script):
1. pm_basic_operations.txt
2. pm_replica_discover_master_crash.txt
3. pm_client_finds_new_master_after_crash.txt
4. pm_write_on_crashed_server.txt

Please give enough time for all scripts to run completely. **Sometimes it might seem that nothing is happening due to a longer wait**.
