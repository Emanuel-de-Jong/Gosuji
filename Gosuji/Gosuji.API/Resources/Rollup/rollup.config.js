import terser from '@rollup/plugin-terser';

const DEBUG = true;

const PAGES = [
    'trainer',
    'josekis',
    'profile',
];

const BASE_INPUT_PATH = "../js/";
const BASE_OUTPUT_PATH_CLIENT = "../../../Gosuji.Client/wwwroot/js/";
const BASE_OUTPUT_PATH_CMS = "../../../Gosuji.CMS/wwwroot/js/";

let plugins = [];
if (!DEBUG) {
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
    outputName='bundle',
    filePath='',
    fileName='custom') =>
({
    input: BASE_INPUT_PATH + filePath + fileName + '.js',
    output: {
        file: outputPath + outputName + '.js',
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
    createGlobalBundle(BASE_OUTPUT_PATH_CLIENT + 'libs/', 'besogo.all', 'libs/', 'besogo.all'),
];

// Page bundles
for (const page of PAGES) {
    config.push(createPageBundle(page));
}

export default config;
