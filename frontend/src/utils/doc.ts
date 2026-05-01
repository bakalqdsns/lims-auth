/**
 * 将 HTML 表格转换为 Word 文档（.doc）并触发下载
 * @param tableHtml 表格的 HTML 字符串
 * @param fileName  下载文件名（不含扩展名）
 */
export function downloadDoc(tableHtml: string, fileName: string): void {
  const html = `
<!DOCTYPE html>
<html xmlns:o="urn:schemas-microsoft-com:office:office"
      xmlns:w="urn:schemas-microsoft-com:office:word"
      xmlns="http://www.w3.org/TR/REC-html40">
<head>
  <meta charset="utf-8" />
  <title>${fileName}</title>
  <style>
    body { font-family: "SimSun", "宋体", sans-serif; font-size: 14px; margin: 20px; }
    table { width: 100%; border-collapse: collapse; margin-bottom: 16px; }
    td, th { border: 1px solid #000; padding: 6px 8px; vertical-align: middle; }
    tr:nth-child(even) { background-color: #f9f9f9; }
    h3 { text-align: center; margin: 10px 0 20px; font-size: 18px; }
    .footer { margin-top: 16px; display: flex; gap: 30px; align-items: center; font-size: 14px; }
    .footer span { white-space: nowrap; }
    input { border: none; border-bottom: 1px solid #000; width: 120px; font-family: inherit; font-size: inherit; background: transparent; }
  </style>
</head>
<body>
${tableHtml}
</body>
</html>`
  const blob = new Blob([html], { type: 'application/msword' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `${fileName}.doc`
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}
