

Single Cassandra instance:  
`docker run --name my-cassandra-1 -m 2g -d cassandra`  

Find the IP address of the container:  
`docker inspect --format='{{ .NetworkSettings.IPAddress }}' my-cassandra-1`  

Connect with CQLS:  
`docker run -it --link my-cassandra-1 --rm cassandra bash -c 'exec cqlsh 172.17.0.2'`  

Add a new container to cluster:  
`docker run --name my-cassandra-2 -m 2g -d -e CASSANDRA_SEEDS="$(docker inspect --format='{{ .NetworkSettings.IPAddress }}' my-cassandra-1)" cassandra`  

Verify the cluster status by running `nodetool status` command:  
`docker exec -i -t my-cassandra-1 bash -c 'nodetool status'`  

Bash scripts:  
cqls.sh: `docker run -it --link my-cassandra-1 --rm cassandra bash -c 'exec cqlsh 172.17.0.2'`  
nodetool.sh: `docker exec -it my-cassandra-1 bash -c "nodetool "$@""`
