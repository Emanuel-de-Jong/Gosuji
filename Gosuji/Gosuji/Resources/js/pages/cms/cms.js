import { cAbuseDetect } from "./cAbuseDetect";
import { cTranslation } from "./cTranslation";
import { stats } from "./stats";

let cmsPage = {};

cmsPage.init = function () {
    custom.switchTheme(false);
};

export { cmsPage, cAbuseDetect, cTranslation, stats };
