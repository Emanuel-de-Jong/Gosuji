import terser from '@rollup/plugin-terser';

const baseInputPath = "../js/";
const baseClientOutputPath = "../../../Gosuji.Client/wwwroot/js/"
const baseCMSOutputPath = "../../../Gosuji.CMS/wwwroot/js/"

const createPageBundle = (filePath, fileName = filePath) => ({
    input: baseInputPath + 'pages/' + filePath + '/' + fileName + '.js',
    output: {
        file: baseClientOutputPath + 'pages/' + filePath + '/bundle.js',
        format: 'es',
        sourcemap: true
    },
    plugins: [
        terser({
            compress: {
                unused: false,       // Prevents removal of unused variables and functions
                side_effects: false  // Avoids dropping code that Terser thinks has no side effects
            },
        })
    ]
});

const createGlobalBundle = (
    outputPath=baseClientOutputPath,
    outputName='bundle.js',
    filePath='',
    fileName='custom.js') =>
({
    input: baseInputPath + filePath + fileName,
    output: {
        file: outputPath + outputName,
        format: 'iife',
        sourcemap: true
    },
    plugins: [
        terser({
            compress: {
                unused: false,
                side_effects: false
            },
        })
    ]
});

let config = [
    // Client globals bundle
    createGlobalBundle(),
    // CMS globals bundle
    createGlobalBundle(baseCMSOutputPath),
    
    // Besogo minify
    createGlobalBundle(baseClientOutputPath + 'libs/', 'besogo.all.js', 'libs/', 'besogo.all.js'),

    // Page bundles
    createPageBundle('trainer'),
    createPageBundle('josekis'),
    createPageBundle('profile'),
];

export default config;
