# .latexmkrc
ensure_path('TEXINPUTS', './report//');
ensure_path('BIBINPUTS', './report//');
ensure_path('BSTINPUTS', './report//');
$emulate_aux_dir = 1;
$lualatex = 'lualatex -shell-escape -interaction=nonstopmode -8bit %O %S';
