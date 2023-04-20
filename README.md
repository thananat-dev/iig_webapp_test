# IIG WebApp Test

### Building and running apps

You can build  apps that package the server to run locally :
* **Docker**:
    * To clone project :
        * `git clone https://github.com/thananat-dev/iig_webapp_test.git`
    
    * To change directory to iig_webapp_test:
        * `cd iig_webapp_test`
     
    * To build it with `docker-compose`:
        * `docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up -d`

Then navigate your browser to [`http://localhost`](http://localhost) to access client app.

### API Docs

WebApp API docs can be found over at http://localhost:7223/swagger/index.html
