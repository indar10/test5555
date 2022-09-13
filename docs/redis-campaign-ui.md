## Redis
Redis (which stands for  **Re**mote  **Di**rectory  **S**erver) is an  [in-memory](https://en.wikipedia.org/wiki/In-memory_database)  data structure store. It is a disk-persistent key-value database with support for  [multiple data structures or data types](https://redis.io/topics/data-types-intro).

This means that while Redis supports mapped key-value-based strings to store and retrieve data (analogous to the data model supported in traditional kinds of databases), it also supports other complex data structures like lists, sets, etc.
The most common use case is simple string/string key-value pairs storage. In Campaign UI we have heavily used Redis to boost the application performance.

## Usage
In Campaign UI, we have implemented the following methods on IRedisCacheHelper interface.

 - `void SetString(string key, string value, bool setExpiration = true);`
 - `string GetString(string key);`
 - `void KeyDeleteWithPrefix(string prefix);`
 
 In order to cache a new Entity, you will need create a new service in the Domain layer [eg: `RedisDivisionCache` ] and just need consume these methods in it.

Currently we are caching the following entities in the application :

 1. Configuration
 2. Look Up
 3. State
 4. User specific data.


 ## Related Posts
 [Redis Setup for Campaign UI](./redis-setup-dev.md)
  
