


## yiyungent/knifehub:latest-amd-full

<!-- docker run -d --restart=always -p 53213:80 -e ASPNETCORE_ENVIRONMENT="Production" -e TZ="Asia/Shanghai" -v ${PWD}/app-filebrowser:/app-filebrowser  --name knifehub yiyungent/knifehub:latest-amd-full -->

```bash
docker run -d --restart=always -p 53213:80 -e ASPNETCORE_ENVIRONMENT="Production" -e TZ="Asia/Shanghai" --name knifehub yiyungent/knifehub:latest-amd-full
```

### KnifeHub

- http://your-domain

### 内置 Web 在线文件管理 filebrowser

- http://your-domain/filebrowser/


> 默认用户名/密码: `admin` / `admin`

### 内置 Selenium 相关

