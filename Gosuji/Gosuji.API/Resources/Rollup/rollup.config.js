import terser from '@rollup/plugin-terser';

const DEBUG = true;

const BASE_INPUT_PATH = "../js/";
const BASE_OUTPUT_PATH_CLIENT = "../../../Gosuji.Client/wwwroot/js/";
const BASE_OUTPUT_PATH_CMS = "../../../Gosuji.CMS/wwwroot/js/";

let plugins = [];
if (DEBUG) {
    
} else {
    plugins.push(terser({
        compress: {
            unused: false,          // Prevents removal of unused variables and functions
            side_effects: false,    // Avoids dropping code that Terser thinks has no side effects
        },
    }));
}

const createPageBundle = (filePath, fileName = filePath) => ({
    input: BASE_INPUT_PATH + 'pages/' + filePath + '/' + fileName + '.js',
    output: {
        file: BASE_OUTPUT_PATH_CLIENT + 'pages/' + filePath + '/bundle.js',
        format: 'es',
        sourcemap: true
    },
    plugins: plugins,
});

const createGlobalBundle = (
    outputPath=BASE_OUTPUT_PATH_CLIENT,
    outputName='bundle.js',
    filePath='',
    fileName='custom.js') =>
({
    input: BASE_INPUT_PATH + filePath + fileName,
    output: {
        file: outputPath + outputName,
        format: 'iife',
        sourcemap: true
    },
    plugins: plugins,
});

let config = [
    // Client globals bundle
    createGlobalBundle(),
    // CMS globals bundle
    createGlobalBundle(BASE_OUTPUT_PATH_CMS),

    // Besogo minify
    createGlobalBundle(BASE_OUTPUT_PATH_CLIENT + 'libs/', 'besogo.all.js', 'libs/', 'besogo.all.js'),

    // Page bundles
    createPageBundle('trainer'),
    createPageBundle('josekis'),
    createPageBundle('profile'),
];

export default config;
