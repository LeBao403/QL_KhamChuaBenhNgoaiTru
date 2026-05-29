$ErrorActionPreference = "Stop"

Set-Location $PSScriptRoot

Write-Host "Neu muon train lai model voi num_walks=80, window_size=5, epochs=50, hay chay rieng:"
Write-Host "python .\retrain_node2vec_report_model.py"
Write-Host ""

Write-Host "1/3 Sinh anh va bieu do cho Chuong 5..."
python .\generate_chapter5_figures.py

Write-Host "2/3 Sinh cac bieu do thuc nghiem bo sung theo model..."
python .\generate_requested_model_charts.py


