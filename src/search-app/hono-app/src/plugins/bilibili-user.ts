import type { BaseResponseModel } from "../types/baseReponseModel";


interface BilibiliUserDataItemModel {
  /**
   * 用户名
   */
  用户名: string;
  /**
   * 头像
   */
  头像: string;

  /**
   * 签名
   */
  签名: string;

  /**
   * 投稿视频个数
   */
  投稿视频数: number;

  /**
   * 粉丝个数
   */
  粉丝数: number;

  /**
   * 等级
   */
  用户等级: number;

  /**
   * https://space.bilibili.com/25057459
   */
  个人空间: string;
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
            用户名: item.uname,
            头像: `${item.upic.replace("//", "http://")}`,
            签名: item.usign,
            投稿视频数: item.videos,
            粉丝数: item.fans,
            用户等级: item.level,
            个人空间: `https://space.bilibili.com/${item.mid}`,
          });
        });
      }
    }
  } catch (error) {
    console.log("error", error);
  }

  return responseModel;
}
