-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 02-06-2024 a las 18:51:11
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `api_inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `id` int(11) NOT NULL,
  `inquilinoId` int(11) NOT NULL,
  `inmuebleId` int(11) NOT NULL,
  `desde` datetime DEFAULT NULL,
  `hasta` datetime DEFAULT NULL,
  `valor` decimal(10,2) NOT NULL DEFAULT 0.00,
  `estado` int(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`id`, `inquilinoId`, `inmuebleId`, `desde`, `hasta`, `valor`, `estado`) VALUES
(3, 5, 5, '2024-03-01 15:12:52', '2025-03-01 15:12:52', 250000.00, 1),
(4, 6, 7, '2024-04-01 18:14:25', '2025-04-01 18:14:25', 35000.00, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` int(11) NOT NULL,
  `direccion` varchar(255) DEFAULT NULL,
  `uso` varchar(255) NOT NULL,
  `tipo` varchar(255) NOT NULL,
  `ambientes` int(11) NOT NULL DEFAULT 1,
  `precio` decimal(10,2) NOT NULL DEFAULT 0.00,
  `propietarioId` int(11) NOT NULL,
  `estado` tinyint(1) NOT NULL,
  `imagen` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `direccion`, `uso`, `tipo`, `ambientes`, `precio`, `propietarioId`, `estado`, `imagen`) VALUES
(4, 'los nogales 715', 'Residencial', 'Casa', 2, 230000.00, 4, 1, '/Uploads\\imagen_9.jpg'),
(5, 'av. los duraznos 954', 'Residencial', 'Casa', 3, 300000.00, 4, 1, '/Uploads\\imagen_6.jpg'),
(6, 'colon 124', 'Residencial', 'Departamento', 6, 200000.00, 4, 0, '/Uploads\\imagen_28.jpg'),
(7, 'Carlos gardel 345', 'Comercial', 'Local', 4, 350000.00, 4, 0, '/Uploads\\imagen_7.jpg'),
(8, 'parque industrial sur 546', 'Comercial', 'Deposito', 8, 7500000.00, 4, 0, '/Uploads\\imagen_8.jpg'),
(9, '9 de julio 619', 'Residencial', 'Casa', 6, 1790000.00, 4, 0, '/Uploads\\imagen_9.jpg'),
(29, 'Carlo ', 'Residencial', 'Casa', 2, 5500000.00, 4, 0, '/Uploads\\imagen_29.jpg'),
(30, 'Rivadavia 430', 'Comercial', 'Local', 7, 4500000.00, 4, 1, '/Uploads\\imagen_30.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` int(11) NOT NULL,
  `dni` bigint(20) NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `apellido` varchar(255) NOT NULL,
  `lugarDeTrabajo` varchar(255) NOT NULL,
  `email` varchar(160) DEFAULT NULL,
  `telefono` varchar(160) DEFAULT NULL,
  `nombreGarante` varchar(255) NOT NULL,
  `telefonoGarante` varchar(255) NOT NULL,
  `estado` int(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `dni`, `nombre`, `apellido`, `lugarDeTrabajo`, `email`, `telefono`, `nombreGarante`, `telefonoGarante`, `estado`) VALUES
(1, 21564879, 'nahuel', 'molina', 'municipalidad de san luis', 'nahuelM@mail.com', '2664123456', 'diego costas', '03512645789', 1),
(2, 25612457, 'juan roman', 'riquelme', 'Bombonera', 'jr10@mail.com', '011246788454', 'diego martinez', '01124545655', 1),
(3, 34660111, 'pol', 'fernandez', 'jugador de boca juniors', 'polF@gmail.com', '011223333334', 'luis advincula', '011245678811', 1),
(5, 35260182, 'luis', 'miguel', 'oficina', 'luismiguel@gmail.com', '2664102340', 'juan gabriel', '2664586798', 1),
(6, 20567890, 'jonas', 'gutierrez', 'fabrica de caramelos', 'jonasG@gmail.com', '2664123597', 'jorge jesus', '03514879564', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `numero` int(11) NOT NULL,
  `contratoId` int(11) NOT NULL,
  `fecha` datetime DEFAULT NULL,
  `importe` decimal(10,2) NOT NULL DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`id`, `numero`, `contratoId`, `fecha`, `importe`) VALUES
(7, 1, 3, '2024-03-01 15:17:37', 250000.00),
(8, 2, 3, '2024-04-09 15:42:05', 250000.00),
(9, 3, 3, '2024-05-09 15:43:24', 250000.00),
(10, 1, 4, '2024-04-10 18:17:52', 35000.00),
(11, 2, 4, '2024-05-10 18:20:31', 35000.00);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `id` int(11) NOT NULL,
  `nombre` varchar(255) NOT NULL,
  `apellido` varchar(255) NOT NULL,
  `dni` bigint(20) NOT NULL,
  `telefono` varchar(160) DEFAULT NULL,
  `email` varchar(160) DEFAULT NULL,
  `password` varchar(160) DEFAULT NULL,
  `avatar` varchar(160) DEFAULT NULL,
  `estado` int(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`id`, `nombre`, `apellido`, `dni`, `telefono`, `email`, `password`, `avatar`, `estado`) VALUES
(3, 'Wilson', 'Ortiz', 34660252, '2664899839', 'wilson@gmail.com', 'V3eS9jJaOwO8EzrO6aD1B9sGI3TGGd4jnG0hTIn22R0=\r\n', '/uploads/perfil3d.jpg', 1),
(4, 'Wilson ', 'Ortis', 34660347, '2664899839', 'wilgon@gmail.com', 'BI+lgbZKLwd/YTHQiA2jLiu0XCELB8uwymqj5dSg0VQ=', '\\uploads\\perfil3d.jpg', 1),
(5, 'carlos', 'menem', 16257189, '0112564897', 'menem@gmail.com', 'kGasKDd3dgvATHT7qctJCjxm+GP/ewcnP2VvIM6d4z4=', '\\uploads\\perfil3d.jpg\r\n', 1),
(6, 'fernanda', 'sosa', 12345678, '266488888', 'fer@gmail.com', 'J76dTfmC56m/cG5XL8o2iSVkxgaWpAHi4zIk5WcvG8E=', '/uploads/perfil3d.jpg\r\n', 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `contratos_inquilinos_inquilinoId` (`inquilinoId`),
  ADD KEY `contratos_inmuebles_inmuebleId` (`inmuebleId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `inmuebles_propietarioId` (`propietarioId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `pagos_contratos_contratoId` (`contratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_inmuebles_inmuebleId` FOREIGN KEY (`inmuebleId`) REFERENCES `inmuebles` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `contratos_inquilinos_inquilinoId` FOREIGN KEY (`inquilinoId`) REFERENCES `inquilinos` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_propietarioId` FOREIGN KEY (`propietarioId`) REFERENCES `propietarios` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_contratos_contratoId` FOREIGN KEY (`contratoId`) REFERENCES `contratos` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
