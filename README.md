# Cassandra Cluster #
A `docker-compose` blueprint that describes a 3 node Cassandra cluster.
It only exposes important Cassandra ports on the seed node to the host
machine. Internally, all of the nodes will be a part of the same Docker
network and will form a cluster using that Docker network.

## Instructions ##
In order to bring up the cluster:
- Use `docker-compose up --build` to see the logs of all the containers
- Use `docker-compose up -d --build` if you want it to run in the background
- Use `docker-compose up -d --force-recreate --build container_name` to rebuild a single container
In order to clean up the cluster, use `docker-compose down`

Find the IP address of the container:  
`docker exec -it my-cassandra bash -c 'ip a'`

Connect with CQLS:  
`docker exec -it my-cassandra bash`  
`cqlsh <IP>`

Verify the cluster status by running `nodetool status` command:  
`docker exec -it my-cassandra bash -c 'nodetool status'`

## Notes ##
You need to make sure that the Docker daemon has enough of resources
otherwise you will encounter exit code 137 (Out of Memory Killer) on
your containers.

When you create a single node cluster and you try to add more nodes to
the cluster, you must add them one by one. This means that you cannot
have multiple nodes join the cluster (by pointing to the seed node) at
the same time. You must add a cluster, wait for it to join the ring and
stabilize before you can begin to add another cluster. This is why you
will see an additional delay on start up between the non-seed nodes.
If you attempt to join a new node whilst stabilization has not yet been
achieved, you will see an error like this:
```
ERROR [main] 2017-08-22 23:19:11,055 CassandraDaemon.java:706 - Exception encountered during startup
java.lang.UnsupportedOperationException: Other bootstrapping/leaving/moving nodes detected, cannot bootstrap while cassandra.consistent.rangemovement is true
```

## Extra ##
```
Just run this from your CLI and it'll drop you in a container with full permissions on the Moby VM. Only works for Moby Linux VM (doesn't work for Windows Containers). Note this also works on Docker for Mac.
```
`docker run -it --rm --privileged --pid=host justincormack/nsenter1`

Create a docker network:
`docker network create --subnet 172.27.0.3/16 pad_network`
# Cassandra CQLSH #
```sql
CREATE  KEYSPACE IF NOT EXISTS my_keyspace
    WITH REPLICATION = {
        'class' : 'SimpleStrategy',
        'replication_factor' : 2
    };
```
```sql
USE my_keyspace;
```
```sql
CREATE TABLE IF NOT EXISTS Movies ( 
   id text,
   name text,
   PRIMARY KEY (id)
);
```
```sql
INSERT INTO Movies (id, name) 
VALUES ('0', 'MovieName') 
IF NOT EXISTS;
```
```sql
SELECT *
FROM Movies;
```