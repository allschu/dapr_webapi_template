## Command to start the project

Go to the project directory and run the following command:

```cmd
dapr run -f .
```

for dashboard:

```cmd
dapr dashboard
```


Pay close attentation to the port number in the launchSettings.json file. 
The port number should be the same as the one in the dapr run command or as described in the
dapr yaml file.


When the project is running, you can test the service by running the following command:

curl http://localhost:55067/v1.0/invoke/[APP_ID]/method/[ENDPOINT_URL]/

see example

```cmd
curl http://localhost:55067/v1.0/invoke/heroes/method/heroes/
```



for more information about Dapr, please visit: https://dapr.io/
https://github.com/dapr/quickstarts/tree/master/service_invocation/csharp/http
