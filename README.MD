# The ultimate queue bot


## TODO list:
- [x] ability to get out of queue
- [ ] skip person on queue
- [ ] delete person by admin
- [ ] add delete admin of queue
- [ ] additerator of queue and show who should go next??

## How to run database from docker
```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<password>" \
   -p 1433:1433 --name sql_perment --hostname sql_permanent \
   -v  $PWD/DBBackup/mssql:/var/opt/mssql \
   -d mcr.microsoft.com/mssql/server:2019-latest
```

### Last updates:
- added docker-compose file
