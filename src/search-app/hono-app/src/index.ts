import { Hono } from "hono";
import { cors } from "hono/cors";
import { env } from "hono/adapter";
import { swaggerUI } from "@hono/swagger-ui";
import game4399account from "./plugins/game4399account";
import bilibiliUser from "./plugins/bilibili-user";

const app = new Hono();
// https://hono.dev/middleware/builtin/cors
app.use("/*", cors());
// https://github.com/honojs/middleware/tree/main/packages/swagger-ui
// Use the middleware to serve Swagger UI at /ui
// app.get('/ui', swaggerUI({ url: '/doc' }))

app.get("/", (c) => {
  return c.json({
    name: "搜索 API",
    version: "1.0.0",
    plugins: [
      {
        name: "4399 用户名查询",
        url: "/api/plugins/game4399account?q={q}",
        test: "/api/plugins/game4399account?q=21434",
      },
      {
        name: "哔哩哔哩用户查询",
        url: "/api/plugins/bilibili-user?q={q}",
        test: "/api/plugins/bilibili-user?q=测试用户",
      },
    ],
  });
});

app.get("/api/plugins/game4399account", async (c) => {
  const q = c.req.query("q");
  if (!q) {
    return c.json({
      code: -1,
      message: "参数错误",
      data: [],
    });
  }
  const res = await game4399account(q);

  return c.json(res);
});

app.get("/api/plugins/bilibili-user", async (c) => {
  const q = c.req.query("q");
  if (!q) {
    return c.json({
      code: -1,
      message: "参数错误",
      data: [],
    });
  }
  const { PLUGINS_BILIBILI_USER_COOKIE } = env<{
    PLUGINS_BILIBILI_USER_COOKIE: string;
  }>(c);
  const res = await bilibiliUser(q, PLUGINS_BILIBILI_USER_COOKIE);

  return c.json(res);
});

export default app;
