# MiniProyecto2

**MiniProyecto2** es una aplicación web desarrollada con **ASP.NET Core MVC**, siguiendo una arquitectura en capas que separa responsabilidades en entidades de dominio, infraestructura, lógica de aplicación y presentación.

## 📁 Estructura del Proyecto

```
MiniProyecto2-main/
├── Domain/              # Entidades de dominio (ej: Milk.cs, BaseEntity.cs)
├── Infrastructure/      # DbContext, repositorios, migraciones EF Core
├── Services/            # Lógica de negocio y servicios comunes
├── Web/                 # Proyecto principal ASP.NET Core MVC
├── Test/UnitTest/       # Proyecto de pruebas unitarias
├── Web.zip              # Copia comprimida del proyecto Web
└── MiniProyecto2.sln    # Archivo de solución
```

## ⚙️ Tecnologías Utilizadas

- .NET 7 / .NET 6 (dependiendo del archivo `.csproj`)
- ASP.NET Core MVC
- Entity Framework Core
- AutoMapper
- SQL Server (a través de EF)
- xUnit (para pruebas)

## 🚀 Cómo Ejecutar el Proyecto

1. **Clona el repositorio:**

```bash
git clone https://github.com/tu_usuario/MiniProyecto2.git
cd MiniProyecto2
```

2. **Restaura dependencias y aplica migraciones:**

```bash
dotnet restore
dotnet ef database update --project Infrastructure
```

3. **Ejecuta la aplicación:**

```bash
dotnet run --project Web
```

4. Abre el navegador en `http://localhost:5000` o como lo indique la consola.

## 🧪 Pruebas

Puedes ejecutar los tests con:

```bash
dotnet test Test/UnitTest
```

## 👤 Autores

- [Tu Nombre Aquí]
- Proyecto académico / demostrativo.

## 📄 Licencia

Este proyecto está bajo licencia [MIT](LICENSE) o la que tú elijas.
