/**
 * 发送提示消息，若用户同意弹出桌面通知，则优先使用桌面通知
 * @param {string} message 消息
 * @param {number} type 成功消息 1, 失败消息 -1
 */
function showMessage(message, type) {
	simpleMsg(message, type);
}

/**
 * 显示消息提示框
 * @param {string} message 消息
 * @param {number} type 成功消息 1, 失败消息 -1
 */
function simpleMsg(message, type) {
	switch (type) {
		case -1:
			toastr.error(message, '失败！');
			break;
		case 1:
			toastr.success(message, '成功！');
			break;
	}
}