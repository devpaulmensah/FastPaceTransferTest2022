CREATE TABLE `users` (
  `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
  `FirstName` longtext CHARACTER SET utf8mb4,
  `LastName` longtext CHARACTER SET utf8mb4,
  `MobileNumber` longtext CHARACTER SET utf8mb4,
  `Password` longtext CHARACTER SET utf8mb4,
  `EmailAddress` longtext CHARACTER SET utf8mb4,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
