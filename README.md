# Jessi Punto de Venta

Jessi Punto de Venta es una aplicación web para administrar ventas, productos y operaciones básicas de una tienda. Está preparada para ejecutarse en IIS y utiliza una base de datos MySQL.

## Requisitos

Antes de instalar la aplicación, el servidor debe contar con:

- Windows Server o Windows 10/11 con IIS habilitado.
- [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- MySQL 8.x o una versión compatible.
- Un usuario de MySQL con permisos sobre la base de datos del sistema.

## Base de datos

El paquete incluye el script `database/CreacionBD.sql`, que crea la base de datos `pos_tiendita` y las tablas necesarias para iniciar.

Para prepararla:

1. Abre MySQL Workbench, phpMyAdmin o una consola de MySQL.
2. Ejecuta el archivo `database/CreacionBD.sql`.
3. Verifica que la base de datos `pos_tiendita` se haya creado correctamente.

## Configuración

Después de descomprimir la aplicación, abre el archivo `appsettings.json` y actualiza la cadena de conexión con los datos reales de tu servidor MySQL:

```json
{
  "ConnectionStrings": {
    "Default": "server=localhost;database=pos_tiendita;user=TU_USUARIO;password=TU_PASSWORD;"
  }
}
```

Cambia `TU_USUARIO` y `TU_PASSWORD` por las credenciales correspondientes. Si MySQL está en otro servidor, reemplaza también `localhost` por la dirección correcta.

## Instalación en IIS

1. Instala el `.NET 8 Hosting Bundle` en el servidor si todavía no está instalado.
2. Descomprime el paquete de la aplicación en una carpeta del servidor, por ejemplo `C:\inetpub\JessiPuntoDeVenta`.
3. Configura la cadena de conexión en `appsettings.json`.
4. Abre el Administrador de IIS.
5. Crea un nuevo sitio o aplicación y apunta la ruta física a la carpeta donde descomprimiste el sistema.
6. Configura el Application Pool con la opción **No Managed Code**.
7. Reinicia el sitio desde IIS.

Al terminar, abre el sitio desde el navegador usando la dirección configurada en IIS.

## Archivos incluidos

El paquete de instalación debe incluir, como mínimo:

- Los archivos publicados de la aplicación.
- El archivo `appsettings.json`.
- El script `database/CreacionBD.sql`.
