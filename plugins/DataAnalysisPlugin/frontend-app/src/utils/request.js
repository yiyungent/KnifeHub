import axios from "axios";

// dev 环境
const devBaseUrl = "http://localhost:5193";
// build 环境
const proBaseUrl = "/";
axios.defaults.baseURL =
  process.env.NODE_ENV === "development" ? devBaseUrl : proBaseUrl;

// axios 请求拦截 - 在发送请求之前做某件事
axios.interceptors.request.use(
  function (request) {
    // 如果有登录状态token的话，则添加到请求头
    if (localStorage.token) {
      // 在 headers 中设置 Authorization 属性放token，token是存在缓存中的
      request.headers.Authorization = `Bearer ${localStorage.token}`;
    }

    return request;
  },
  function (error) {
    return Promise.reject(error);
  }
);

/**
 * 发送http请求
 * @param {String} url 目标rl
 * @param {String} method 方法get,post,put etc.
 * @param {Object} params 对象参数
 * @returns {Promise<unknown>}
 */
const request = function (url, method, params) {
  return new Promise((resolve, reject) => {
    if (method == "get") {
      axios({
        url: url,
        method: "get",
        params: params,
      })
        .then((res) => {
          res.data.status = res.status;
          resolve(res.data);
        })
        .catch((err) => {
          err.data.status = err.status;
          reject(err.data);
        });
    } else {
      axios({
        url: url,
        method: method,
        data: params,
      })
        .then((res) => {
          res.data.status = res.status;
          resolve(res.data);
        })
        .catch((err) => {
          err.data.status = err.status;
          reject(err.data);
        });
    }
  });
};

export default request;
