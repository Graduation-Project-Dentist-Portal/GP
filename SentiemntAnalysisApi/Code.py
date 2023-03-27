from transformers import AutoTokenizer
from transformers import AutoModelForSequenceClassification
from scipy.special import softmax
from flask import Flask, request
from langdetect import detect

app = Flask(__name__)
ENGLISHMODEL = f"cardiffnlp/twitter-roberta-base-sentiment"
ARABICMODEL = f"akhooli/xlm-r-large-arabic-sent"

tokenizer_english = AutoTokenizer.from_pretrained(ENGLISHMODEL)
model_english = AutoModelForSequenceClassification.from_pretrained(ENGLISHMODEL)

tokenizer_arabic = AutoTokenizer.from_pretrained(ARABICMODEL)
model_arabic = AutoModelForSequenceClassification.from_pretrained(ARABICMODEL)


@app.route('/api/get-score', methods=['POST'])
def get_score():
    comment = request.json['comment']
    if detect(comment) == 'en':
        encoded_comment = tokenizer_english(comment, return_tensors='pt')
        output = model_english(**encoded_comment)
        scores = output[0][0].detach().numpy()
        scores = softmax(scores)
        if scores[0] > scores[1] and scores[0] > scores[2]:
            return "-1"
        elif scores[2] > scores[1] and scores[2] > scores[0]:
            return "1"
        else:
            return "0"
    elif detect(comment) == 'ar':
        encoded_comment = tokenizer_arabic(comment, return_tensors='pt')
        output = model_arabic(**encoded_comment)
        scores = output[0][0].detach().numpy()
        scores = softmax(scores)
        if scores[0] > scores[1] and scores[0] > scores[2]:
            return "0"
        elif scores[2] > scores[1] and scores[2] > scores[0]:
            return "1"
        else:
            return "-1"
    else:
        return "Wrong language", 400
