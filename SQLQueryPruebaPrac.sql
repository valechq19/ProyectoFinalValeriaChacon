CREATE DATABASE PruebaPrac;

USE PruebaPrac;

CREATE TABLE Usuario (
    Id INT PRIMARY KEY IDENTITY,
    Usuario NVARCHAR(50) NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL 
)

INSERT INTO Usuario (Usuario, Contrasena)
VALUES ('Valeria123', 'password123')

CREATE TABLE Articulos (
    Id INT PRIMARY KEY IDENTITY(1,1),  
    Codigo VARCHAR(50) NOT NULL,        
    Nombre VARCHAR(100) NOT NULL,       
    Precio DECIMAL(10, 2) NOT NULL,     
    AplicaIVA BIT NOT NULL              
)

CREATE TABLE Factura (
    Id INT PRIMARY KEY IDENTITY(1,1),  
    Codigo VARCHAR(50) NOT NULL,        
    Nombre VARCHAR(100) NOT NULL,       
    Precio DECIMAL(10, 2) NOT NULL,    
	Total DECIMAL(10, 2) NOT NULL,      
    Cantidad INT NOT NULL,              
    Iva BIT NOT NULL                    

--------------------------------------------------------------------------------------

CREATE PROCEDURE AgregarArticulo
    @Codigo VARCHAR(50),
    @Nombre VARCHAR(100),
    @Precio DECIMAL(10, 2),
    @AplicaIVA BIT
AS
BEGIN
    INSERT INTO Articulos (Codigo, Nombre, Precio, AplicaIVA)
    VALUES (@Codigo, @Nombre, @Precio, @AplicaIVA);
END;

EXEC SP_AgregarProductoAFactura @Codigo = 'A001', @Cantidad = 2;


CREATE PROCEDURE SP_AgregarProductoAFactura
(
    @Codigo VARCHAR(50),           
    @Cantidad INT,                 
    @TotalTodo DECIMAL OUTPUT      
)
AS
BEGIN
   
    DECLARE @Nombre VARCHAR(100);
    DECLARE @Precio DECIMAL(10, 2);
    DECLARE @Total DECIMAL(10, 2);
    DECLARE @AplicaIVA BIT;
    DECLARE @CantidadExistente INT;
    DECLARE @TotalExistente DECIMAL(10, 2);

  
    SELECT 
        @Nombre = Nombre,
        @Precio = Precio,
        @AplicaIVA = AplicaIVA
    FROM 
        Articulos
    WHERE 
        Codigo = @Codigo;

    
    IF @Nombre IS NULL
    BEGIN
        RAISERROR('Producto no encontrado', 16, 1);
        RETURN;
    END

    
    IF @AplicaIVA = 1
    BEGIN
        SET @Precio = @Precio * 1.13;
    END

    
    SET @Total = @Cantidad * @Precio;

    
    SELECT 
        @CantidadExistente = Cantidad,
        @TotalExistente = Total
    FROM 
        Factura
    WHERE 
        Codigo = @Codigo;

   
    IF @CantidadExistente IS NOT NULL
    BEGIN
        
        UPDATE Factura
        SET 
            Cantidad = @CantidadExistente + @Cantidad, 
            Total = @TotalExistente + @Total
        WHERE 
            Codigo = @Codigo;
    END
    ELSE
    BEGIN
        
        INSERT INTO Factura (Codigo, Nombre, Precio, Cantidad, Total, Iva)
        VALUES (@Codigo, @Nombre, @Precio, @Cantidad, @Total, @AplicaIVA);
    END

 
    SELECT @TotalTodo = SUM(Total) FROM Factura;
END;
GO

----------------------------------------------------------------
INSERT INTO Articulos (Codigo, Nombre, Precio, AplicaIVA) VALUES
('C001', 'Mesa de Comedor', 150.00, 1),
('C002', 'Silla de Oficina', 75.50, 1),
('C003', 'Sofá de 3 Plazas', 600.00, 1),
('C004', 'Estante de Libros', 120.00, 0),
('C005', 'Lámpara de Techo', 45.99, 1);