import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import "normalize.css";
import App from "./App";
import reportWebVitals from "./reportWebVitals";
import env from "./env.js";

const VERSION = env.version;
const GIT_HASH = "";
console.log(
  `${"\n"} %c data-analysis-plugin-frontend-app v${VERSION} ${GIT_HASH} %c https://github.com/yiyungent/KnifeHub/tree/main/plugins/DataAnalysisPlugin ${"\n"}${"\n"}`,
  "color: #fff; background: #030307; padding:5px 0;",
  "background: #ff80ab; padding:5px 0;"
);

import { FluentProvider, teamsLightTheme } from "@fluentui/react-components";

const root = ReactDOM.createRoot(
  document.getElementById("root") as HTMLElement
);
root.render(
  <FluentProvider theme={teamsLightTheme} style={{ height: "100%" }}>
    <App />
  </FluentProvider>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
