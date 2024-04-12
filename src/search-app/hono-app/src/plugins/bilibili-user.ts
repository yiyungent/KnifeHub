import type { BaseResponseModel } from "../types/baseReponseModel";


interface BilibiliUserDataItemModel {
  /**
   * 用户名
   */
  uname: string;
  /**
   * 头像
   */
  upic: string;

  /**
   * 签名
   */
  usign: string;

  /**
   * 投稿视频个数
   */
  videos: number;

  /**
   * 粉丝个数
   */
  fans: number;

  /**
   * 等级
   */
  level: number;

  /**
   * https://space.bilibili.com/25057459
   */
  url: string;
}

export default async function query(
  query: string,
  cookie: string,
): Promise<BaseResponseModel<BilibiliUserDataItemModel>> {
  const responseModel: BaseResponseModel<BilibiliUserDataItemModel> = {
    code: -1,
    message: "失败",
    data: [],
  };
  try {
    const res = await fetch(
      `https://api.bilibili.com/x/web-interface/wbi/search/type?` +
        new URLSearchParams({
          category_id: "",
          search_type: "bili_user",
          ad_resource: "5646",
          __refresh__: "true",
          page: "1",
          page_size: "36",
          platform: "pc",
          highlight: "1",
          single_column: "0",
          keyword: query.trim(),
          qv_id: "drhuMbLFmLakkilY2NVwvZlCArwuxzso",
          source_tag: "3",
          order_sort: "0",
          user_type: "0",
          dynamic_offset: "0",
          web_location: "1430654",
          w_rid: "c69cf3c4599ba1fcd0429dcfad83f2e8",
          wts: "1712952282",
        }),
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          "User-Agent": `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36`,
          Referer:
            `https://search.bilibili.com/upuser?` +
            new URLSearchParams({
              keyword: query.trim(),
              search_source: "5",
            }),
          Accept: "application/json, text/plain, */*",
          "Accept-Language": "zh-CN,zh;q=0.9,en;q=0.8",
          Origin: "https://search.bilibili.com",
          "Sec-Ch-Ua": `"Google Chrome";v="123", "Not:A-Brand";v="8", "Chromium";v="123"`,
          Cookie: `${cookie}`
        },
      }
    );

    console.log("res", res);

    if (res.ok) {
      responseModel.code = 1;
      responseModel.message = "成功";
      const resJson: any = await res.json();
      if (
        resJson.code === 0 &&
        resJson.data?.result &&
        resJson.data?.result.length > 0
      ) {
        resJson.data.result.forEach((item: any) => {
          responseModel.data.push({
            uname: item.uname,
            upic: `${item.upic.replace("//", "http://")}`,
            usign: item.usign,
            videos: item.videos,
            fans: item.fans,
            level: item.level,
            url: `https://space.bilibili.com/${item.mid}`,
          });
        });
      }
    }
  } catch (error) {
    console.log("error", error);
  }

  return responseModel;
}
