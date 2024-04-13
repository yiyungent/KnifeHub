import type { BaseResponseModel } from "../types/baseReponseModel";

interface Gamme4399AccountDataItemModel {
  /**
   * 4399账号
   */
  账号: string;
  /**
   * 此账号在4399平台不存在
   * 此账号在4399平台已存在
   */
  提示: string;

  /**
   * https://www.4399.com
   */
  来源: string;
}

/**
 * https://u.4399.com/anquan/pwd/?from=u&sfrom=
 * @param query 
 * @returns 
 */
export default async function query(
  query: string
): Promise<BaseResponseModel<Gamme4399AccountDataItemModel>> {
  const responseModel: BaseResponseModel<Gamme4399AccountDataItemModel> = {
    code: -1,
    message: "失败",
    data: [],
  };
  try {
    // 创建一个新的 Date 对象
    const date = new Date();
    // 获得以毫秒为单位的时间戳
    const timeStamp = date.getTime();
    const res = await fetch(
      `https://u.4399.com/anquan/pwd/?` +
        new URLSearchParams({
          _c: "verify",
          _a: "userAllowFind",
          userName: query.trim(),
          jsoncallback: "jQuery110206197225005545752_1712947018019",
          _: `${timeStamp}`,
        }),
      {
        method: "GET",
        headers: {
          "User-Agent":
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36",
          Referer: "https://u.4399.com/anquan/pwd/?from=u&sfrom=",
          "Content-Type": "application/x-www-form-urlencoded",
          Accept: `text/javascript, application/javascript, application/ecmascript, application/x-ecmascript, */*; q=0.01`,
          "X-Requested-With": "XMLHttpRequest",
          "Sec-Ch-Ua-Platform": `"Windows"`,
        },
      }
    );

    if (res.ok) {
      responseModel.code = 1;
      responseModel.message = "成功";
      const text = await res.text();
      // decoded: jQuery110206197225005545752_1712947018007({status: false, msg: "此账号在4399平台不存在"})
      // jQuery110206197225005545752_1712947018007({"status":false,"msg":"\u6b64\u8d26\u53f7\u57284399\u5e73\u53f0\u4e0d\u5b58\u5728"})
      // jQuery110206197225005545752_1712947018007({"status":true})
      if (text.indexOf(`"status":true`) > 0) {
        responseModel.message = "成功";
        responseModel.data = [
          {
            账号: query.trim(),
            提示: "此账号在4399平台已存在",
            来源: "https://www.4399.com",
          },
        ];
      } else if (text.indexOf(`"status":false`) > 0) {
        responseModel.message = `成功: 此账号 (${query.trim()}) 在4399平台不存在`;
        // 不存在就不需要展示列表了
        responseModel.data = [];
      } else {
        responseModel.code = -1;
        responseModel.message = "未知错误";
      }
    }
  } catch (error) {}

  return responseModel;
}
