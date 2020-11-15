-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        5.7.29 - MySQL Community Server (GPL)
-- 服务器操作系统:                      Win32
-- HeidiSQL 版本:                  10.3.0.5855
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- 导出 auth_center 的数据库结构
CREATE DATABASE IF NOT EXISTS `auth_center` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `auth_center`;

-- 导出  表 auth_center.uinfo 结构
CREATE TABLE IF NOT EXISTS `uinfo` (
  `uid` int(11) unsigned NOT NULL AUTO_INCREMENT COMMENT '玩家唯一的UID号',
  `unick` varchar(32) NOT NULL DEFAULT '' COMMENT '玩家的昵称',
  `usex` int(8) NOT NULL DEFAULT '0' COMMENT '0:男  1:女',
  `uface` int(8) NOT NULL DEFAULT '0' COMMENT '系统默认头像,自定义头像后面再加',
  `uname` varchar(32) NOT NULL DEFAULT '0' COMMENT '玩家账号名称',
  `upwd` varchar(32) DEFAULT '' COMMENT '玩家密码的MD5值',
  `phone` varchar(16) NOT NULL DEFAULT '' COMMENT '玩家电话号码',
  `email` varchar(64) NOT NULL DEFAULT '' COMMENT '玩家的email',
  `address` varchar(128) NOT NULL DEFAULT '' COMMENT '玩家的地址',
  `uvip` int(8) NOT NULL DEFAULT '0' COMMENT '玩家vip等级',
  `vip_end_time` int(32) NOT NULL DEFAULT '0' COMMENT '玩家vip到期的时间戳',
  `is_guest` int(8) NOT NULL DEFAULT '0' COMMENT '标志改账号是否为游客账号',
  `guest_key` varchar(64) NOT NULL DEFAULT '' COMMENT '游客账号的唯一key',
  `status` int(8) NOT NULL DEFAULT '0' COMMENT '0正常，其他的根据需求来定',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=utf8 COMMENT='存放我们的玩家信息';

-- 数据导出被取消选择。

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
