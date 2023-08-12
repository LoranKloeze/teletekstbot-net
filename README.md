![alt text](https://github.com/LoranKloeze/teletekstbot-net/actions/workflows/test.yml/badge.svg)
![alt text](https://github.com/LoranKloeze/teletekstbot-net/actions/workflows/test_and_deploy.yml/badge.svg)
# Teletekstbot
This bot tries to detect newly pushed Teletekst pages from the NOS
and posts them to Bluesky and Mastodon.

## How it works
The bot loops over a range of Teletekst numbers and for each number it:
1. Fetches the title and contents from the NOS using their unofficial API
2. Checks if it has already posted the combination of page number and title (skips to 1 if it has)
3. Creates a screenshot from the Teletekst webpage
4. Posts the page to Bluesky and Mastodon

Each post on Bluesky and Mastodon contains:
- The title of the page
- The page number
- A link to the NOS
- A screenshot of the Teletekst page

## Run it yourself
### Requirements
- A Redis server, configure the host in one of the `appsettings.*.json` files
- A headless Chrome browser, fastest way is to run it as a Docker instance: 
  https://hub.docker.com/r/browserless/chrome. Again, configure the host in one
  of the `appsettings.*.json` files.
- Copy `.env.sample` to `.env` and use your credentials for Bluesky and Mastodon
- Run the solution in Visual Studio, Rider or your favourite IDE

## Admin

The application contains a very light admin mode. The only thing it does, is clear 
the 'already seen pages' cache in Redis. Run `dotnet run clear` in 
the `TeletekstBot.BotService` project.
