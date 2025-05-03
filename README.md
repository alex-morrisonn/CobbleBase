# CoBBlE Platform

## Features
- 

## Requirements
* Kuberneties
* ASP.NET Core (URL)
*

## Getting Started
1. 
2.  

## Modules
Developers have the albility to create and install modules that allow to interact with the CoBBlE platform using a global API system.

## MiniKube Start
1. Start MiniKube
```bash
minikube start
```
2. Apply the kubernetes configuration
```bash
kubectl apply -f deployment.yaml -f clusterroles.yaml -f clusterrolesbindinggs.yaml -f serviceaccount.yaml
```
3. Expose the service
```bash
kubectl expose deployment cobbleapp --type=NodePort --port=8080
```
4. Get the URL
```bash
url=$(minikube service cobbleapp --url)
```
5. Post to the API
```bash
curl -X POST -H "Content-Type: application/json" -d '"712346/app2"' $url/deployment/deploy
```
6. Expose the deployment
```bash
kubectl expose deployment plugin-712346-app2 --type=NodePort --port=5000
```
7. Get the URL
```bash
plugin_url=$(minikube service plugin-712346-app2 --url)
```
8. Get the data
```bash
curl $plugin_url/get_org
```
