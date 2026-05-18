# .latexmkrc
ensure_path('TEXINPUTS', './/');
ensure_path('BIBINPUTS', './/');
ensure_path('BSTINPUTS', './/');
$emulate_aux_dir = 1;
$lualatex = 'lualatex -shell-escape -interaction=nonstopmode -8bit %O %S';
