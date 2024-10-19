SELECT Products.[Product Name], Categories.[Category Name]
FROM Products
INNER JOIN Categories ON Products.[Product Category] = Categories.[Category Name]
WHERE Categories.[Category Name] = "Fruits"