# Proyecto Backend - API RESTful con .NET 8

Este proyecto consiste en un **API RESTful** desarrollada con **C# y .NET 8** para gestionar productos en un catálogo y un carrito de compras.

## Requisitos Previos

- **.NET 8** o superior instalado.
- **SQL Server** para gestionar la base de datos.
- **Visual Studio 2022 o superior** recomendado para desarrollo.

## Configuración de la Base de Datos

La base de datos utilizada es **SQL Server**. La información del catálogo de productos y el carrito de compras está almacenada en la base de datos.

### 1. Importar la Base de Datos

Debes importar el archivo `ShoppingCartDb.bacpac` en tu instancia de SQL Server. Esto se puede hacer mediante SQL Server Management Studio (SSMS) o utilizando Azure Data Studio. El archivo `.bacpac` contiene toda la data necesaria para la base de datos, incluyendo las tablas para productos y carrito de compras.

### 2. Configuración del `appsettings.json`

En el archivo `appsettings.json` de tu proyecto, debes configurar la conexión a la base de datos de SQL Server. La sección `ConnectionStrings` debe tener la ruta de conexión adecuada a tu base de datos. Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=ShoppingCartDb;User Id=TU_USUARIO;Password=TU_CONTRASEÑA;"
  },
  ...
}

# Resumen de lo que se ha hecho:

- **Gestión de productos**: Endpoint para obtener el listado de productos.
- **Carrito de compras**: Endpoints para añadir productos, obtener productos en el carrito y eliminar productos.
- **Base de datos SQL Server**: Se ha configurado una base de datos con las tablas necesarias para productos y carrito de compras.
- **Autenticación básica**: Para gestionar el carrito de compras de manera independiente para cada usuario.
