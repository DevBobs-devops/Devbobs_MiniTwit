# .latexmkrc
$out_dir = 'report/build';
$aux_dir = 'report/build';
$emulate_aux_dir = 1;
ensure_path('TEXINPUTS', './report/build//');
$lualatex = 'lualatex -shell-escape -interaction=nonstopmode -8bit %O %S';
