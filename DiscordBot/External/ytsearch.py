import sys
import yt_dlp
import requests
import json

def search(query):
    ydl_opts = {
        "format": "bestaudio",
        "noplaylist": "False",
        "quiet": "True"
    }
    
    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        try:
            requests.get(query)  # If this succeeds, query is a URL
        except:
            # Search on YouTube
            info = ydl.extract_info(f"ytsearch:{query}", download=False)["entries"][0]
        else:
            # Direct link
            info = ydl.extract_info(query, download=False)
            

        return info, info["url"]

if __name__ == "__main__":
    if len(sys.argv) < 2:
        sys.exit(1)

    query = sys.argv[1]
    info, url = search(query)

    result = {
        "Url": url,
        "Title": info['title']
    }

    print(json.dumps(result))