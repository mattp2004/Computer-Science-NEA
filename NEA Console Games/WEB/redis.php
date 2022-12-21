<?php 
   echo "hello";
   //Connecting to Redis server on localhost 
   $redis = new Redis([
       'host' => '127.0.0.1',
       'port' => 6379,
       'connectTimeout' => 2.5,
       'auth' => ['phpredis', '*2p74#7BpejD'],
       'ssl' => ['verify_peer' => false],
       'backoff' => [
           'algorithm' => Redis::BACKOFF_ALGORITHM_DECORRELATED_JITTER,
           'base' => 500,
           'cap' => 750,
       ],
   ]);
   echo "t";
   $redis->connect('127.0.0.1', 6379); 
   echo "Connection to server sucessfully"; 
   //check whether server is running or not 
   echo "Server is running: ".$redis->ping(); 
?>