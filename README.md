# Gosuji
A webapp to get feedback from AI, while playing against AI. This is THE method to learn the new AI intuition and sequences.

[Blog about the features, creation process and roadmap.](https://gosuji.blogspot.com/2025/06/the-process-of-making-gosuji-new-way-of.html)

**Active Development:** 2022-12-21 - 2025-06-09<br>
**Last Change:** 2025-06-09<br>

| | |
| :---: | :---: |
| ![](/Screenshots/1-Trainer.png) | ![](/Screenshots/2-Trainer-Light.png) |
| ![](/Screenshots/3-Learn.png) | ![](/Screenshots/4-Profile.png) |
| ![](/Screenshots/5-Settings-General.png) | ![](/Screenshots/6-Subscriptions.png) |
| ![](/Screenshots/7-Contact.png) | ![](/Screenshots/8-Josekis.png) |

## Setup
1. Find the KataGo model version in `KataGoVersion.MODEL`, download it from [KataGo Training](https://katagotraining.org/networks/) and put it in `Resources/KataGo/Models`.
2. Run `npm install` in `Gosuji.API/Resources/Rollup`.
3. Change the API and Client launch settings from `https` to `http`.
4. Make a new profile that first starts the API and then the Client.
 