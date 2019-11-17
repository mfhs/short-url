# Short-Url
URL Shortening Service (.NetCore/C#)

Will update more gradually...


## Swagger is integrated for easy web browser accessible UI


## Actions
- Shorten a long URL: Given a valid long URL, api will return a short url.

- Expand a short URL: Given a valid short url (shorten by this api before), will return the original long url.

## The Api also confirms the following:
- The shortened URL (at least the relative URL path) will be shorter in length than the provided URL (minimum length: 3 & maximum length: 8).

- If same long URL is provided, the same short URL will be returned.


## Example:
Say, If we provide following long url--

"https://www.google.com/search?source=hp&ei=DdDQXfmsGIH0vgT1pZOwCg&q=.net+core+3.0&oq=.net+Core+&gs_l=psy-ab.3.0.0l10.4465.9759..11425...0.0..0.136.1320.7j6......0....1..gws-wiz.....0..0i131.gAlFufDT8Y8" 

the api may return a unique short url like this ->  "http://s.my/kmRgXmmT"


## Currently URLs Supported by the following Regex for the time beign (we may update as per required):
    
    "^(?:(https?|ftps?):\\/\\/)?[\\w.-]+(?:\\.[\\w\\.-]+)+[\\w\\-\\._~:/?#[\\]@!\\$&'\\(\\)\\*\\+,;=.]+$"


## Supported Short-Url format:
- https://s.my/{shortCode}
- http://s.my/{shortCode}
