SELECT Customers.Name, Orders.OrderName
FROM Customers
INNER JOIN Orders ON Customers.[Order] = Orders.OrderName;