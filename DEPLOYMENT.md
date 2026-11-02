# Deployment Guide

## 🚀 Deploying to GitHub Pages (Frontend)

### Automatic Deployment (GitHub Actions)
1. Push your code to the `main` branch
2. GitHub Actions will automatically build and deploy to GitHub Pages
3. Your site will be available at: `https://<username>.github.io/<repo-name>/`

### Manual Deployment
```bash
# Build the project
npm run build

# Install gh-pages package
npm install -D gh-pages

# Deploy to gh-pages branch
npx gh-pages -d dist
```

### Configure for GitHub Pages
Add to `vite.config.js`:
```javascript
export default defineConfig({
  base: '/<repo-name>/',
  // ... rest of config
});
```

---

## 🌐 Deploying Backend (.NET)

### Option 1: Azure App Service
1. Create an Azure App Service (Linux/Windows)
2. Set up a SQL Database
3. Update connection string in `appsettings.json`
4. Deploy via GitHub Actions or Azure CLI:
```bash
az webapp up --name sharpcuts-api --resource-group sharpcuts-rg
```

### Option 2: Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/SharpCuts.API/SharpCuts.API.csproj", "SharpCuts.API/"]
RUN dotnet restore "SharpCuts.API/SharpCuts.API.csproj"
COPY backend/ .
WORKDIR "/src/SharpCuts.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SharpCuts.API.dll"]
```

### Option 3: AWS Elastic Beanstalk
```bash
# Install AWS EB CLI
pip install awsebcli

# Initialize and deploy
eb init sharpcuts-api
eb create production
```

---

## 🔧 Environment Variables

### Frontend (.env)
```
VITE_API_URL=https://your-api-url.com/api
```

### Backend (appsettings.Production.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=SharpCutsDB;User Id=sa;Password=your-password;"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-here",
    "Issuer": "SharpCutsAPI",
    "Audience": "SharpCutsClient"
  }
}
```

---

## 📋 Pre-deployment Checklist

- [ ] Update API URL in frontend environment
- [ ] Configure CORS for production domain
- [ ] Set up database migrations
- [ ] Configure SSL certificates
- [ ] Set up monitoring/logging
- [ ] Configure environment variables
- [ ] Test all endpoints in production

---

## 🔗 Useful Links

- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/)
- [Vite Deployment Guide](https://vitejs.dev/guide/static-deploy.html)
- [.NET Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/)
