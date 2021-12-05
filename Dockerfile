apiVersion: apps/v1
kind: Deployment
metadata:
  name: inventory
  labels:
    app: inventory
spec:
  replicas: 1
  selector:
    matchLabels:
      app: inventory
  template:
    metadata:
      labels:
        app: inventory
    spec:
      containers:
        - name: inventory
          image: githubtestingreg.azurecr.io/inventorymanagement
          ports:
            - containerPort: 80
          resources:
            limits:
              cpu: "200m"
              memory: 200Mi
            requests:
              cpu: "100m"
              memory: 100Mi
---
apiVersion: v1
kind: Service
metadata:
  name: inventory
  labels:
    app: inventory
spec:
  ports:
  - port: 8080
    targetPort: 80
  selector:
    app: inventory
---
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: inventory-gateway
spec:
  selector:
    istio: ingressgateway
  servers:
    - port:
        number: 80
        name: http-web
        protocol: HTTP
      hosts:
        - "*"
---
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: inventory-vservice
spec:
  hosts:
    - "*"
  gateways:
    - inventory-gateway
  http:
   - route:
      - destination:
          host: inventory
          port:
            number: 8080
