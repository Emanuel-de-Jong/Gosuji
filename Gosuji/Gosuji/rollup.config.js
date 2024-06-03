import fs from 'fs';
import path from 'path';
import terser from '@rollup/plugin-terser';

const createPageBundle = (filePath, fileName = filePath) => ({
    input: 'Resources/js/pages/' + filePath + '/' + fileName + '.js',
    output: {
        file: 'wwwroot/js/pages/' + filePath + '/bundle.js',
        format: 'es',
        sourcemap: false
    },
    plugins: [
        terser()
    ]
});

let config = [
    createPageBundle('josekis'),
    createPageBundle('cms'),
];

function getJavaScriptFiles(dir, fileList = []) {
    const files = fs.readdirSync(dir);
    files.forEach(file => {
        const filePath = path.join(dir, file);
        const fileStat = fs.statSync(filePath);
        if (fileStat.isDirectory()) {
            getJavaScriptFiles(filePath, fileList);
        } else if (filePath.endsWith('.js')) {
            fileList.push(filePath);
        }
    });
    return fileList;
}

const files = getJavaScriptFiles('Resources/js/');

config = [ ...config, ...files.map(file => {
    const outputFileName = file.replace('Resources\\js\\', 'wwwroot\\js\\');

    return {
        input: file,
        output: {
            file: outputFileName,
            format: 'es',
            sourcemap: false
        },
        plugins: [
            terser({
                mangle: false,
                compress: {
                    unused: false,
                    side_effects: false
                }
            })
        ]
    };
})];

export default config;
