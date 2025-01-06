# DB Synopse
 Synopsis Project for Database for Developers

# Setup
First make sure that inside the OrderService, you make the file "run-ef-database-update" to a LF line break types.

Make sure docker is running.

Then input the following command into the console:

```
docker compose up
```
To access and use the system open swagger:
```
localhost:8086/swagger/index.html
```
# Test Run
**CreateProduct:** First use CreateProduct POST request to make dummy data, inside mongoDB
```
{
  "productId": 1,
  "name": "Random Produck 1",
  "stock": 200
}
```
![CreateProduct](https://github.com/AsbjrnJacobsen/DB-Synopse/blob/main/PNG/CreateProduct.png)

**CreateOrder:** Now you can test create orders by using CreateOrder POST
```
{
  "orderDto": {
    "productId": 1,
    "visableFlag": true
  },
  "quantity": 1
}
```
![CreateOrder](https://github.com/AsbjrnJacobsen/DB-Synopse/blob/main/PNG/CreateOrder.png)

# See synchronization
To se the synchronization you can tjek with the GetAllOrders and GetAllProducts EndPoints

**GetAllOrders:**
![GetAllOrders](https://github.com/AsbjrnJacobsen/DB-Synopse/blob/main/PNG/GetAllOrders.png)

**GetAllProducts:**
![GetAllProducts](https://github.com/AsbjrnJacobsen/DB-Synopse/blob/main/PNG/GetAllProducts.png)