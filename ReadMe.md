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

### Install Dapr

install dapr on your kubernetes cluster

```
dapr init -k
```


## Implement Prometheus and Grafana with the following

### Install Prometheus

```
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm install dapr-prom prometheus-community/prometheus
```


Ensure Prometheus is running in your cluster.

https://docs.dapr.io/operations/observability/metrics/prometheus/


### Install Grafana

Make sure that prometheus is installed according to the above

Add grafana to your Helm repo

```
helm repo add grafana https://grafana.github.io/helm-charts
helm repo update
```

Install grafana

```
helm install grafana grafana/grafana
```


https://docs.dapr.io/operations/observability/metrics/grafana/


for more information about Dapr, please visit: 
https://dapr.io/
https://github.com/dapr/quickstarts/tree/master/service_invocation/csharp/http
