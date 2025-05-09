# Trados Studio QuickInfo
Trados Studio plugin which extends default Tell Me functionality to provde various information like units conversion, math calculation, airport codes, Unicode description and more.

## Prerequisits

* Trados Studio 2019

## Currently supported answers

1. Unit conversions:
   1. Weight: pounds to kg and vice versa (`190 pounds`, `200lb`, `43 kg in lb`)
   1. Temperature: fahrenheit/celsius (`75f`, `75 f`, `75 fahrenheit`, `23c`, `23 c`, `23 c to f`)
   1. Distance: `12 miles`, `100 miles in km`
   1. Speed
   1. Volume
   1. Area: `1670 sq.ft`
   1. Fuel efficiency (`29mpg`)
   1. Curency conversion. Following currencies are supported: EUR, USD, GBP, RUB, CZK, ILS, CAD, CHF, RON, AUD, PLN, HUF, JPY, CNY
1. Hex/decimal (just type a number)
1. List of colors: `color`, `colors`
1. Color conversions - convert an RGB triplet to Hex color or hex color to RGB (`23 145 175`, `#eeaaf0`)
1. Factor an integer `2520`
1. Generate a random Guid `guid`
1. My IP address `ip`
1. Math/arithmetic expressions `(24 * 365) * 4 - 1`, `22/7 - pi`, `rnd`, `Random(100)`
1. Quick sum/average/product of a list of numbers, sort a list (separated by space or comma) `3 7 21 3 3 2 11`
1. Unicode: type a codepoint to see what char it is: `U+2021` `\u3333`
1. Url decode: `2%20%2B%203` shows the original string (fun exercise: `2%2525252520%252525252B%25252525205`)

This is based on the [QuickInfo library](https://github.com/KirillOsenkov/QuickInfo).
