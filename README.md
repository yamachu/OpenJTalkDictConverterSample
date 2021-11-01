SharpOpenJTalkでUser辞書使ったり

https://github.com/neologd/mecab-ipadic-neologd

をOpenJTalkで使えるようにコンバートするサンプルスクリプト

コードには一部OpenJTalkの内部コードが含まれる
当該コードのLICENSEは同梱している[COPYING](./COPYING)を参照

### Try

```
$ dotnet build
$ dotnet run -- open_jtalk_dic_utf_8-1.11を解凍したDirecotry sampledict.csv hts_voice_nitech_jp_atr503_m001-1.05/nitech_jp_atr503_m001.htsvoice
$ # これでOpenJTalkのsystem辞書では形態素解析できない GITHUB のような単語が正しく読まれる
```

https://github.com/neologd/mecab-ipadic-neologd

で辞書を作りたい場合は、mecab-user-dict-seed.20200910.csv.xzなどを解凍し、sampledict.csvの代わりに読ませると、open_jtalk_dic_utf_8-1.11を解凍したDirecotryにuser.dicが吐き出される

作成された辞書で正しく読まれない場合は、文脈IDを削除したりコストを下げるなどの処理を行うと良い

ref: https://taku910.github.io/mecab/dic.html
