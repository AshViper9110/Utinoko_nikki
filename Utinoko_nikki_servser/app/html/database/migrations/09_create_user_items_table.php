<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('user_items', function (Blueprint $table) {
            $table->id();

            $table->unsignedBigInteger('elder_user_id');
            $table->unsignedBigInteger('items_id');

            $table->integer('quantity')->default(0);

            $table->dateTime('created_at')->useCurrent();
            $table->dateTime('updated_at')
                ->useCurrent()
                ->useCurrentOnUpdate();

            // 高齢者ユーザーとの外部キー制約
            $table->foreign('elder_user_id')
                ->references('id')
                ->on('elder_users');

            // アイテムとの外部キー制約
            $table->foreign('items_id')
                ->references('id')
                ->on('items');

            // 同じユーザーが同じアイテムを重複して登録することを防ぐ
            $table->unique(['elder_user_id', 'items_id']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('user_items');
    }
};
