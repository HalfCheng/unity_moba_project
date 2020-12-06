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
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8 COMMENT='存放我们的玩家信息';

-- 数据导出被取消选择。


-- 导出 moba_game 的数据库结构
CREATE DATABASE IF NOT EXISTS `moba_game` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `moba_game`;

-- 导出  表 moba_game.login_bonues 结构
CREATE TABLE IF NOT EXISTS `login_bonues` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '领取奖励的唯一id',
  `uid` int(11) DEFAULT NULL COMMENT '用户的uid',
  `bonues` int(11) DEFAULT '0' COMMENT '奖励的数目',
  `status` int(11) NOT NULL DEFAULT '0' COMMENT '是否已经领取，0未领取，1已领取',
  `bonues_time` int(11) DEFAULT NULL COMMENT '发放奖励的时间',
  `days` int(11) DEFAULT '0' COMMENT '连续登录天数',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COMMENT='登录奖励管理';

-- 数据导出被取消选择。

-- 导出  表 moba_game.sys_msg 结构
CREATE TABLE IF NOT EXISTS `sys_msg` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `msg` varchar(256) DEFAULT '0' COMMENT '系统消息的内容',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。

-- 导出  表 moba_game.ugame 结构
CREATE TABLE IF NOT EXISTS `ugame` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '全局唯一id',
  `uid` int(11) NOT NULL COMMENT '用户uid',
  `uchip` int(11) NOT NULL DEFAULT '0' COMMENT '用户的金币数目',
  `uchip2` int(11) NOT NULL DEFAULT '0' COMMENT '用户的金币数目扩展',
  `uchip3` int(11) NOT NULL DEFAULT '0' COMMENT '用户的金币数目扩展',
  `uvip` int(11) NOT NULL DEFAULT '0' COMMENT '用户在本游戏中的等级',
  `uvip_endtime` int(11) NOT NULL DEFAULT '0' COMMENT 'vip结束时间',
  `udata1` int(11) NOT NULL DEFAULT '0' COMMENT '用户在游戏中的道具1',
  `udata2` int(11) NOT NULL DEFAULT '0' COMMENT '用户在游戏中的道具2',
  `udata3` int(11) NOT NULL DEFAULT '0' COMMENT '用户在游戏中的道具3',
  `uexp` int(11) NOT NULL DEFAULT '0' COMMENT '用户的经验值',
  `ustatus` int(11) NOT NULL DEFAULT '0' COMMENT '0正常，其他为不正常',
  `is_robot` int(11) NOT NULL DEFAULT '0' COMMENT '0为普通用户，1为机器人',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='存放我们玩家在moba这个游戏中的玩家的游戏数据，主要的游戏数据有：\r\n金币，其他货币，道具，游戏中的vip等级，账号状态，玩家的经验\r\nuid来标识玩家的，id最为自增长的唯一id号';

-- 数据导出被取消选择。

-- 导出  表 moba_game.ulevel 结构
CREATE TABLE IF NOT EXISTS `ulevel` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ulevel` int(11) NOT NULL DEFAULT '0',
  `uexp` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- 数据导出被取消选择。

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
