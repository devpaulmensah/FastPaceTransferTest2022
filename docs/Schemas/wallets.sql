CREATE TABLE `wallets` (
  `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
  `UserId` longtext CHARACTER SET utf8mb4,
  `Balance` decimal(65,30) NOT NULL,
  `Currency` longtext CHARACTER SET utf8mb4,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
